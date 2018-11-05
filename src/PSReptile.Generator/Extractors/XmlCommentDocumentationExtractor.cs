using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using PSReptile.Maml;

namespace PSReptile.Extractors
{
    using Utilities;

    /// <summary>
    ///     Cmdlet documentation extractor that obtains documentation from XML documentation comments.
    /// </summary>
    public class XmlCommentDocumentationExtractor
        : IDocumentationExtractor
    {
        /// <summary>
        ///     Cached XML for documentation comments.
        /// </summary>
        readonly Dictionary<Assembly, XmlDoc> _documentationCache = new Dictionary<Assembly, XmlDoc>();

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

            XmlDoc assemblyDoc = GetAssemblyDocumentation(cmdletType);
            if (assemblyDoc == null)
                return null;

            return assemblyDoc.GetSummary(cmdletType);
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

            XmlDoc assemblyDoc = GetAssemblyDocumentation(cmdletType);
            if (assemblyDoc == null)
                return null;

            return assemblyDoc.GetRemarks(cmdletType);
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

            XmlDoc assemblyDoc = GetAssemblyDocumentation(parameterProperty);
            if (assemblyDoc == null)
                return null;

            return assemblyDoc.GetSummary(parameterProperty);
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
            
            var assemblyDoc = GetAssemblyDocumentation(cmdletType);
            if(assemblyDoc == null)
                return null;
            var rawExamples = assemblyDoc.GetExamples(cmdletType);
            
            var examples = new List<CommandExample>();
            foreach (var example in rawExamples)
            {
                var para = example.Elements("para");
                
                var descriptions = para.Where(e => e.Attribute("type")?.Value == "description").Select(e => MamlGenerator.ToParagraphs(e.Value.Trim()));
                var description = new List<string>();
                foreach(var d in descriptions)
                    description.AddRange(d);
                
                var remarks = para.Where(e => e.Attribute("type")?.Value != "description").Select(e => MamlGenerator.ToParagraphs(e.Value.Trim()));
                var remark = new List<string>();
                foreach(var r in remarks)
                    remark.AddRange(r);
                
                examples.Add(
                    new CommandExample
                    {
                        Title = example.Element("title")?.Value.Trim() ?? "Example",
                        Description = description,
                        Code = string.Join("\n", example.Elements("code")?.InDocumentOrder().Select(e => e.Value)),
                        Remarks = remark
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
            
            var assemblyDoc = GetAssemblyDocumentation(cmdletType);
            if(assemblyDoc == null)
                return null;
            var returnElements = assemblyDoc.GetReturns(cmdletType);

            var returnValues = new List<CommandValue>();
            foreach(var returnElement in returnElements)
            {
                var see = returnElement.Element("sees");
                var paras = returnElement.Elements("para")?.Select(e => MamlGenerator.ToParagraphs(e?.Value?.Trim())).ToList() ?? new List<List<string>>();
                var description = new List<string>();

                if (see == null && paras.Count == 0)
                {
                    description.AddRange(MamlGenerator.ToParagraphs(returnElement.Value?.Trim()));
                } else
                {
                    foreach (var list in paras)
                    {
                        description.AddRange(list);
                    }
                }
                
                if(see == null || !see.HasAttributes)
                {
                    var first = description[0];
                    description.RemoveAt(0);
                    returnValues.Add(
                        new CommandValue
                        {
                            DataType = new DataType { Name = first },
                            Description = description
                        }
                    );
                    continue;
                }

                returnValues.Add(
                    new CommandValue
                    {
                        DataType = new DataType
                        {
                            Name = see.Attribute("cref")?.Value?.Trim() ?? string.Empty,
                            Uri = see.Attribute("uri")?.Value?.Trim(),
                            Description = MamlGenerator.ToParagraphs(see.Value?.Trim())
                        },
                        Description = description
                    }
                );
            }

            return returnValues;
        }

        /// <summary>
        ///     Retrieve the documentation (if any) for the specified property's declaring type's assembly.
        /// </summary>
        /// <param name="property">
        ///     The property.
        /// </param>
        /// <returns>
        ///     The documentation XML (as an <see cref="XmlDoc"/>), or <c>null</c> if no documentation was found for the property's declaring type's assembly.
        /// </returns>
        XmlDoc GetAssemblyDocumentation(PropertyInfo property)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));

            return GetAssemblyDocumentation(property.DeclaringType);
        }

        /// <summary>
        ///     Retrieve the documentation (if any) for the specified type's assembly.
        /// </summary>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <returns>
        ///     The documentation XML (as an <see cref="XmlDoc"/>), or <c>null</c> if no documentation was found for the type's assembly.
        /// </returns>
        XmlDoc GetAssemblyDocumentation(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return GetAssemblyDocumentation(type.GetTypeInfo());
        }

        /// <summary>
        ///     Retrieve the documentation (if any) for the specified type's assembly.
        /// </summary>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <returns>
        ///     The documentation XML (as an <see cref="XmlDoc"/>), or <c>null</c> if no documentation was found for the type's assembly.
        /// </returns>
        XmlDoc GetAssemblyDocumentation(TypeInfo type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return GetAssemblyDocumentation(type.Assembly);
        }

        /// <summary>
        ///     Retrieve the documentation (if any) for the specified assembly.
        /// </summary>
        /// <param name="assembly">
        ///     The target assembly.
        /// </param>
        /// <returns>
        ///     The documentation XML (as an <see cref="XmlDoc"/>), or <c>null</c> if no documentation was found for the target assembly.
        /// </returns>
        XmlDoc GetAssemblyDocumentation(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            XmlDoc documentation;
            if (!_documentationCache.TryGetValue(assembly, out documentation))
            {
                FileInfo documentationFile = new FileInfo(
                    Path.ChangeExtension(assembly.Location, ".xml")
                );
                if (!documentationFile.Exists)
                    return null;

                using (Stream documentationStream = documentationFile.OpenRead())
                {
                    documentation = new XmlDoc(
                        XDocument.Load(documentationStream)
                    );
                }
                _documentationCache.Add(assembly, documentation);
            }

            return documentation;
        }
    }
}