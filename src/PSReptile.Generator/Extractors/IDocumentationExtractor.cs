using System.Collections.Generic;
using System.Reflection;
using PSReptile.Maml;

namespace PSReptile.Extractors
{
    /// <summary>
    ///     Represents a mechanism for extracting Cmdlet documentation from reflected types and members.
    /// </summary>
    public interface IDocumentationExtractor
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
        string GetCmdletSynopsis(TypeInfo cmdletType);

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
        string GetCmdletDescription(TypeInfo cmdletType);

        /// <summary>
        ///     Extract the description for a Cmdlet parameter.
        /// </summary>
        /// <param name="parameterDescription">
        ///     The property that represents the parameter.
        /// </param>
        /// <returns>
        ///     The description, or <c>null</c> if no description could be extracted by this extractor.
        /// 
        ///     An empty string means a description was extracted, but the description is empty (this is legal).
        /// </returns>
        string GetParameterDescription(PropertyInfo parameterDescription);

        /// <summary>
        ///     Extract the examples for a Cmdlet parameter.
        /// </summary>
        /// <param name="cmdletType">
        ///     The CLR type that implements the Cmdlet.
        /// </param>
        /// <returns>
        ///     A list of examples, which may be empty or null.
        /// </returns>
        List<CommandExample> GetCmdletExamples(TypeInfo cmdletType);

        /// <summary>
        ///     Extract the return values for a Cmdlet.
        /// </summary>
        /// <param name="cmdletType">
        ///     The CLR type that implements the Cmdlet.
        /// </param>
        /// <returns>
        ///     A list of values, which may be empty or null.
        /// </returns>
        List<CommandValue> GetCmdletReturnValues(TypeInfo cmdletType);
    }
}