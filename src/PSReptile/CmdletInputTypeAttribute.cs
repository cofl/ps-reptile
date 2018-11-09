using System;

namespace PSReptile
{
    /// <summary>
    ///     Defines an input type for a Cmdlet.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class CmdletInputTypeAttribute
        : Attribute
    {
        /// <summary>
        ///     Defines an input type for a Cmdlet.
        /// </summary>
        /// <param name="typeName">The name of the input type; defaults to "None" if null or empty.</param>
        public CmdletInputTypeAttribute(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
                typeName = "None";
            Name = typeName;
        }

        /// <summary>
        ///     Defines an input type for a Cmdlet.
        /// </summary>
        /// <param name="type">The input type.</param>
        public CmdletInputTypeAttribute(Type type) : this(type?.Name)
        {
            Type = type;
        }

        /// <summary>
        ///     The description of the input type.
        /// </summary>
        public string Description { get; }

        /// <summary>
        ///     If the input type is a CLR type.
        /// </summary>
        public bool IsCLRType
            => Type != null;

        /// <summary>
        ///     The name of the input type.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        ///     The Uri of the input type.
        /// </summary>
        public string Uri { get; }

        /// <summary>
        ///     The CLR type of the input type.
        /// </summary>
        public Type Type { get; private set; } = null;
    }
}