using System;

namespace PSReptile
{
    /// <summary>
    ///     Provides an example in the help content for a Cmdlet.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class CmdletExampleAttribute
        : Attribute
    {
        /// <summary>
        ///     Provides an example in the help content for a Cmdlet.
        /// </summary>
        /// <param name="title">The title of the example.</param>
        /// <param name="code">The code of the example.</param>
        /// <param name="remarks">The remarks of the example.</param>
        public CmdletExampleAttribute(string title, string code, string remarks)
        {
            if (title == null)
                throw new ArgumentNullException(nameof(title));
            if (code == null)
                throw new ArgumentNullException(nameof(code));
            if (remarks == null)
                throw new ArgumentNullException(nameof(remarks));

            Title = title;
            Code = code;
            Remarks = remarks;
        }

        /// <summary>
        ///     The title of the example.
        /// </summary>
        public string Title { get; }

        /// <summary>
        ///     An introduction to the example.
        /// </summary>
        public string Description { get; }

        /// <summary>
        ///     The example code.
        /// </summary>
        public string Code { get; }


        /// <summary>
        ///     An introduction to the example.
        /// </summary>
        public string Remarks { get; }
    }
}