using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Reflection;
using PSReptile.Maml;

namespace PSReptile.Extractors
{
    /// <summary>
    ///     Cmdlet documentation extractor that obtains documentation from custom attributes.
    /// </summary>
    public class ReflectionDocumentationExtractor
        : IDocumentationExtractor
    {
        /// <summary>
        ///     Extract the synopsis for a Cmdlet.
        /// </summary>
        /// <param name="cmdletType">
        ///     The CLR type that implements the Cmdlet.
        /// </param>
        /// <returns>
        ///     The synopsis, or <c>null</c> if no synopsis could be extracted by this extractor.
        /// 
        ///     An empty string means a synopsis was extracted, but the synopsis is empty (this is legal).
        /// </returns>
        public string GetCmdletSynopsis(TypeInfo cmdletType)
        {
            if (cmdletType == null)
                throw new ArgumentNullException(nameof(cmdletType));

            CmdletSynopsisAttribute synopsisAttribute = cmdletType.GetCustomAttribute<CmdletSynopsisAttribute>();
            if (synopsisAttribute == null)
                return null;

            return synopsisAttribute.Synopsis?.Trim();
        }

        /// <summary>
        ///     Extract the synopsis for a Cmdlet.
        /// </summary>
        /// <param name="cmdletType">
        ///     The CLR type that implements the Cmdlet.
        /// </param>
        /// <returns>
        ///     The description, or <c>null</c> if no description could be extracted by this extractor.
        /// 
        ///     An empty string means a description was extracted, but the description is empty (this is legal).
        /// </returns>
        public string GetCmdletDescription(TypeInfo cmdletType)
        {
            if (cmdletType == null)
                throw new ArgumentNullException(nameof(cmdletType));

            CmdletDescriptionAttribute descriptionAttribute = cmdletType.GetCustomAttribute<CmdletDescriptionAttribute>();
            if (descriptionAttribute == null)
                return null;

            return descriptionAttribute.Description?.Trim();
        }

        /// <summary>
        ///     Extract the description for a Cmdlet parameter.
        /// </summary>
        /// <param name="parameterProperty">
        ///     The property that represents the parameter.
        /// </param>
        /// <returns>
        ///     The description, or <c>null</c> if no description could be extracted by this extractor.
        /// 
        ///     An empty string means a description was extracted, but the description is empty (this is legal).
        /// </returns>
        public string GetParameterDescription(PropertyInfo parameterProperty)
        {
            if (parameterProperty == null)
                throw new ArgumentNullException(nameof(parameterProperty));

            ParameterAttribute descriptionAttribute = parameterProperty.GetCustomAttribute<ParameterAttribute>();
            if (descriptionAttribute == null)
                return null;

            // TODO: Handle resource-based messages (with locale).

            return descriptionAttribute.HelpMessage?.Trim();
        }

        /// <summary>
        ///     Extract the examples for a Cmdlet parameter.
        /// </summary>
        /// <param name="cmdletType">
        ///     The CLR type that implements the Cmdlet.
        /// </param>
        /// <returns>
        ///     A list of example, which may be empty.
        /// </returns>
        public List<CommandExample> GetCmdletExamples(TypeInfo cmdletType)
        {
            if (cmdletType == null)
                throw new ArgumentNullException(nameof(cmdletType));
            
            var attributes = cmdletType.GetCustomAttributes<CmdletExampleAttribute>();
            if (attributes == null)
                return null;
            
            var examples = new List<CommandExample>();
            foreach (var attribute in attributes)
            {
                examples.Add(
                    new CommandExample
                    {
                        Title = attribute.Title,
                        Description = MamlGenerator.ToParagraphs(attribute.Description),
                        Code = attribute.Code,
                        Remarks = MamlGenerator.ToParagraphs(attribute.Remarks)
                    }
                );
            }

            return examples;
        }

        /// <summary>
        ///     Extract the return values for a Cmdlet.
        /// </summary>
        /// <param name="cmdletType">
        ///     The CLR type that implements the Cmdlet.
        /// </param>
        /// <returns>
        ///     A list of values, which may be empty or null.
        /// </returns>
        public List<CommandValue> GetCmdletReturnValues(TypeInfo cmdletType)
        {
            if (cmdletType == null)
                throw new ArgumentNullException(nameof(cmdletType));

            var returnValuesAttributes = cmdletType.GetCustomAttributes<OutputTypeAttribute>();
            if (returnValuesAttributes == null)
                return null;
            
            var returnValues = new List<CommandValue>();
            foreach (var attribute in returnValuesAttributes)
            {
                foreach (var type in attribute.Type)
                {
                    returnValues.Add(new CommandValue { DataType = new DataType { Name = type.Name } });
                }
            }

            return returnValues;
        }
    }
}
