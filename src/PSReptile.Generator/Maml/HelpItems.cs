using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace PSReptile.Maml
{
    /// <summary>
    ///     The root ("helpItems") XML element in a Powershell MAML help file.
    /// </summary>
    [XmlRoot("helpItems", Namespace = Constants.XmlNamespace.Root)]
    public class HelpItems
    {
        /// <summary>
        ///     No idea what the point of this atribute is.
        /// </summary>
        [XmlAttribute("schema")]
        public string Schema { get; set; } = "maml";

        /// <summary>
        ///     Command documentation.
        /// </summary>
        [XmlElement("command", Namespace = Constants.XmlNamespace.Command)]
        public List<Command> Commands { get; set; } = new List<Command>();

        /// <summary>
        ///     Write the help to the specified <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="writer">
        ///     <see cref="TextWriter"/>
        /// </param>
        public void WriteTo(TextWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            var settings = new XmlWriterSettings
            {
                NewLineChars = "\n"
            };

            using(var xmlWriter = XmlWriter.Create(writer, settings))
                WriteTo(xmlWriter);
        }

        /// <summary>
        ///     Write the help to the specified <see cref="XmlWriter"/>.
        /// </summary>
        /// <param name="writer">
        ///     <see cref="XmlWriter"/>
        /// </param>
        public void WriteTo(XmlWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            new XmlSerializer(GetType()).Serialize(
                writer, this, Constants.XmlNamespace.GetStandardPrefixes()
            );
        }
    }
}