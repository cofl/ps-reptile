using System;
using System.Xml.Serialization;

namespace PSReptile.Maml
{
    /// <summary>
    ///     Supported Powershell pipeline input types for Cmdlet parameters.
    /// </summary>
    [Flags]
    public enum PipelineInputType
    {
        /// <summary>
        ///     Parameter does not take its value from the pipeline.
        /// </summary>
        [XmlEnum("false")]
        None = 0,

        /// <summary>
        ///     Parameter can take its value from the pipeline.
        /// </summary>
        [XmlEnum("true (ByValue)")]
        ByValue = 1,

        /// <summary>
        ///     Parameter can take its value from a property of the same name on objects in the pipeline.
        /// </summary>
        [XmlEnum("true (ByPropertyName)")]
        ByPropertyName = 2,

        /// <summary>
        ///     Parameter can take its value from the pipeline, or from a property of the same name on objects in the pipeline.
        /// </summary>
        [XmlEnum("true (ByPropertyName, ByValue)")]
        ByPropertyNameAndValue = 3,

        /// <summary>
        ///     Parameter can take its value from remaining arguments on the pipeline.
        /// </summary>
        [XmlEnum("true (FromRemainingArguments)")]
        FromRemainingArguments = 4,
        
        /// <summary>
        ///     Parameter can take its value from the pipeline, or from remaining arguments on the pipeline.
        /// </summary>
        [XmlEnum("true (ByValue, FromRemainingArguments)")]
        FromRemainingArgumentsAndByValue = 5,

        /// <summary>
        ///     Parameter can take its value from remaining arguments on the pipeline, or from a property of the same name on objects in the pipeline.
        /// </summary>
        [XmlEnum("true (ByPropertyName, FromRemainingArguments)")]
        FromRemainingArgumentsAndByPropertyName = 6,

        /// <summary>
        ///     Parameter can take its value from the pipeline, or from a property of the same name on objects in the pipeline, or from remaining arguments on the pipeline
        /// </summary>
        [XmlEnum("true (ByPropertyName, ByValue, FromRemainingArguments)")]
        FromRemainingArgumentsAndByPropertyNameAndValue = 7
    }
}