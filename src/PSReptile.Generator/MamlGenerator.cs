using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation;
using System.Reflection;

namespace PSReptile
{
    using Extractors;
    using Maml;

    /// <summary>
    ///     Generates MAML from Powershell binary modules and the Cmdlets they contain.
    /// </summary>
    public class MamlGenerator
    {
        /// <summary>
        ///     Create a new MAML generator with the default documentation extractors (reflection, falling back to XML doc comments).
        /// </summary>
        public MamlGenerator()
            : this(new ReflectionDocumentationExtractor(), new XmlCommentDocumentationExtractor())
        {
        }

        /// <summary>
        ///     Create a new MAML generator with the specified documentation extractors
        /// </summary>
        /// <param name="documentationExtractors">
        ///     The documentation extractors, in the order they should be used.
        /// </param>
        public MamlGenerator(params IDocumentationExtractor[] documentationExtractors)
            : this((IEnumerable<IDocumentationExtractor>)documentationExtractors)
        {
        }

        /// <summary>
        ///     Create a new MAML generator with the specified documentation extractors
        /// </summary>
        /// <param name="documentationExtractors">
        ///     The documentation extractors, in the order they should be used.
        /// </param>
        public MamlGenerator(IEnumerable<IDocumentationExtractor> documentationExtractors)
        {
            if (documentationExtractors == null)
                throw new ArgumentNullException(nameof(documentationExtractors));
            if (documentationExtractors.Any(item => item == null))
                throw new ArgumentNullException(nameof(documentationExtractors), "List had null element.");

            DocumentationExtractors.AddRange(documentationExtractors);
        }

        /// <summary>
        ///     Extractors for documentation.
        /// </summary>
        List<IDocumentationExtractor> DocumentationExtractors { get; } = new List<IDocumentationExtractor>();

        /// <summary>
        ///     Generate MAML documentation for the specified module.
        /// </summary>
        /// <param name="moduleAssembly">
        ///     The assembly that implements the module.
        /// </param>
        /// <returns>
        ///     A <see cref="Command"/> representing the Cmdlet documentation.
        /// </returns>
        public HelpItems Generate(Assembly moduleAssembly)
        {
            if (moduleAssembly == null)
                throw new ArgumentNullException(nameof(moduleAssembly));

            HelpItems help = new HelpItems();
            help.Commands.AddRange(
                Reflector.GetCmdletTypes(moduleAssembly).Select(
                    cmdletType => Generate(cmdletType)
                )
            );

            return help;
        }

        /// <summary>
        ///     Generate MAML documentation for the specified Cmdlet.
        /// </summary>
        /// <param name="cmdletType">
        ///     The CLR type that implements the Cmdlet.
        /// </param>
        /// <returns>
        ///     A <see cref="Command"/> representing the Cmdlet documentation.
        /// </returns>
        public Command Generate(Type cmdletType)
        {
            if (cmdletType == null)
                throw new ArgumentNullException(nameof(cmdletType));

            if (!Reflector.IsCmdlet(cmdletType))
                throw new ArgumentException($"'{cmdletType.FullName}' does not implement a Cmdlet (must be public, non-abstract, derive from '{typeof(Cmdlet).FullName}', and be decorated with '{typeof(CmdletAttribute).FullName}').", nameof(cmdletType));

            var cmdletTypeInfo = cmdletType.GetTypeInfo();
            var cmdletAttribute = cmdletTypeInfo.GetCustomAttribute<CmdletAttribute>();
            Debug.Assert(cmdletAttribute != null, "cmdletAttribute != null");

            var commandHelp = new Command
            {
                Details =
                {
                    Name = $"{cmdletAttribute.VerbName}-{cmdletAttribute.NounName}",
                    Synopsis = ToParagraphs(GetCmdletSynopsis(cmdletTypeInfo) ?? String.Empty),
                    Verb = cmdletAttribute.VerbName,
                    Noun = cmdletAttribute.NounName
                },
                Description = ToParagraphs(GetCmdletDescription(cmdletTypeInfo) ?? String.Empty)
            };

            var parameterSets = new Dictionary<string, SyntaxItem>();
            foreach (var property in cmdletType.GetProperties().OrderBy(property => property.CanRead))
            {
                if (!Reflector.IsCmdletParameter(property, out var parameterAttributes))
                    continue;

                // TODO: Add support for localised help from resources.

                var parameter = GetParameter(cmdletType, property, parameterAttributes, out var parameterSetNames);
                commandHelp.Parameters.Add(parameter);

                foreach(var name in parameterSetNames)
                {
                    if (!parameterSets.TryGetValue(name, out var parameterSetSyntax))
                    {
                        parameterSetSyntax = new SyntaxItem { CommandName = commandHelp.Details.Name };
                        parameterSets.Add(name, parameterSetSyntax);
                    }

                    parameterSetSyntax.Parameters.Add(parameter);
                }
            }

            foreach (var parameterSetName in parameterSets.Keys.OrderBy(name => name))
            {
                commandHelp.Syntax.Add(parameterSets[parameterSetName]);
            }

            commandHelp.Examples.AddRange(GetCmdletExamples(cmdletTypeInfo).OrderBy(example => example.Title));
            commandHelp.ReturnValues.AddRange(GetCmdletReturnValues(cmdletTypeInfo));
            commandHelp.InputTypes.AddRange(GetCmdletInputTypes(cmdletTypeInfo));

            return commandHelp;
        }

        Parameter GetParameter(Type cmdletType, PropertyInfo property, IEnumerable<ParameterAttribute> parameterAttributes, out IList<string> parameterSetNames)
        {
            var parameter = new Parameter
            {
                Name = property.Name,
                Aliases = string.Join(", ", property.GetCustomAttribute<AliasAttribute>()?.AliasNames ?? new List<string> { "none" }),
                Description = GetParameterDescription(property),
                Value =
                {
                    IsMandatory = property.PropertyType != typeof(SwitchParameter),
                    DataType = PowerShellIfyTypeName(property.PropertyType),
                },
                SupportsGlobbing = property.GetCustomAttribute<SupportsWildcardsAttribute>() != null
            };

            var defaultValue = GetParameterDefaultValue(cmdletType, property);
            if(defaultValue != null)
                parameter.DefaultValue = defaultValue;
            
            var lastAttribute = parameterAttributes.Last();
            if(lastAttribute.Position != int.MinValue)
                parameter.Position = lastAttribute.Position.ToString();
            
            var parameterSetList = new LinkedList<string>();
            foreach(var attribute in parameterAttributes)
            {
                parameter.IsMandatory = parameter.IsMandatory || attribute.Mandatory;
                parameter.SupportsPipelineInput |= attribute.ValueFromPipeline ? PipelineInputType.ByValue : PipelineInputType.None;
                parameter.SupportsPipelineInput |= attribute.ValueFromPipelineByPropertyName ? PipelineInputType.ByPropertyName : PipelineInputType.None;
                parameter.SupportsPipelineInput |= attribute.ValueFromRemainingArguments ? PipelineInputType.FromRemainingArguments : PipelineInputType.None;
                parameterSetList.AddFirst(attribute.ParameterSetName);
            }
            parameterSetNames = parameterSetList.ToList();

            return parameter;
        }

        /// <summary>
        ///     Get the synopsis for the specified Cmdlet.
        /// </summary>
        /// <param name="cmdletType">
        ///     The CLR type that implements the Cmdlet.
        /// </param>
        /// <returns>
        ///     The synopsis, or <c>null</c> if none of the registered documentation extractors was able to provide a synopsis for the Cmdlet.
        /// </returns>
        string GetCmdletSynopsis(TypeInfo cmdletType)
        {
            if (cmdletType == null)
                throw new ArgumentNullException(nameof(cmdletType));

            foreach (IDocumentationExtractor extractor in DocumentationExtractors)
            {
                string synopsis = extractor.GetCmdletSynopsis(cmdletType);
                if (synopsis != null)
                    return synopsis;
            }

            return null;
        }

        /// <summary>
        ///     Get the description for the specified Cmdlet.
        /// </summary>
        /// <param name="cmdletType">
        ///     The CLR type that implements the Cmdlet.
        /// </param>
        /// <returns>
        ///     The description, or <c>null</c> if none of the registered documentation extractors was able to provide a description for the Cmdlet.
        /// </returns>
        string GetCmdletDescription(TypeInfo cmdletType)
        {
            if (cmdletType == null)
                throw new ArgumentNullException(nameof(cmdletType));

            foreach (IDocumentationExtractor extractor in DocumentationExtractors)
            {
                string description = extractor.GetCmdletDescription(cmdletType);
                if (description != null)
                    return description;
            }

            return null;
        }

        /// <summary>
        ///     Get the description for the specified Cmdlet.
        /// </summary>
        /// <param name="parameterProperty">
        ///     The CLR type that implements the Cmdlet.
        /// </param>
        /// <returns>
        ///     The description, or <c>null</c> if none of the registered documentation extractors was able to provide a description for the Cmdlet.
        /// </returns>
        List<string> GetParameterDescription(PropertyInfo parameterProperty)
        {
            if (parameterProperty == null)
                throw new ArgumentNullException(nameof(parameterProperty));

            foreach (IDocumentationExtractor extractor in DocumentationExtractors)
            {
                var description = extractor.GetParameterDescription(parameterProperty);
                if (description != null)
                    return description;
            }

            return new List<string>();
        }

        string GetParameterDefaultValue(Type cmdletType, PropertyInfo propertyInfo)
        {
            var temp = cmdletType.GetConstructor(new Type[0])?.Invoke(new object[0]) ?? null;
            if(temp is null)
                return null;
            
            var value = propertyInfo.GetValue(temp);
            (temp as IDisposable)?.Dispose();

            return value?.ToString() ?? null;
        }

        IEnumerable<CommandExample> GetCmdletExamples(TypeInfo cmdletType)
        {
            if (cmdletType == null)
                throw new ArgumentNullException(nameof(cmdletType));
            
            foreach (IDocumentationExtractor extractor in DocumentationExtractors)
            {
                var examples = extractor.GetCmdletExamples(cmdletType);
                if (examples != null && examples.Count > 0)
                    return examples;
            }

            return Enumerable.Empty<CommandExample>();
        }

        IEnumerable<CommandValue> GetCmdletReturnValues(TypeInfo cmdletType)
        {
            if (cmdletType == null)
                throw new ArgumentNullException(nameof(cmdletType));
            
            foreach (var extractor in DocumentationExtractors)
            {
                var values = extractor.GetCmdletReturnValues(cmdletType);
                if (values != null && values.Count > 0)
                    return values;
            }

            return Enumerable.Empty<CommandValue>();
        }

        IEnumerable<CommandValue> GetCmdletInputTypes(TypeInfo cmdletType)
        {
            if (cmdletType == null)
                throw new ArgumentNullException(nameof(cmdletType));
            
            foreach(var extractor in DocumentationExtractors)
            {
                var values = extractor.GetCmdletInputTypes(cmdletType);
                if (values != null && values.Count > 0)
                    return values;
            }

            return Enumerable.Empty<CommandValue>();
        }

        /// <summary>
        ///     Split text into paragraphs.
        /// </summary>
        /// <param name="text">
        ///     The text to split.
        /// </param>
        /// <returns>
        ///     A list of paragraphs.
        /// </returns>
        public static List<string> ToParagraphs(string text)
        {
            if (text == null)
                text = String.Empty;

            return text.Split(
                        separator: new[] { "\r\n", "\n" },
                        options: StringSplitOptions.None
                    ).Select(line => line.Trim())
                    .ToList();
        }

        /// <summary>
        ///     Powershellifies a type's name (using the simple name if in the System namespace).
        /// </summary>
        /// <param name="type">
        ///     Ths type to get the name of.
        /// </param>
        /// <returns>
        ///     The type's name according to PowerShell's convention.
        /// </returns>
        public static string PowerShellIfyTypeName(Type type)
        {
            // This is a PowerShell convention.
            if (type.Namespace == "System")
            {
                return type.Name;
            } else
            {
                return type.FullName;
            }
        }
    }
}