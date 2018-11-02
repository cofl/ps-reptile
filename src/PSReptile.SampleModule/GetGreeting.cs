using System.Management.Automation;

namespace PSReptile.SampleModule
{
    /// <summary>
    ///     A simple Cmdlet that outputs a greeting to the pipeline.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "Greeting")]
    [CmdletSynopsis("A simple Cmdlet that outputs a greeting to the pipeline")]
    [CmdletDescription(@"
        This Cmdlet works with greetings.
        It gets them.
        I can't see how to make it any clearer than that.
    ")]
    public class GetGreeting
        : Cmdlet
    {
        /// <summary>
        ///     The name of the person to greet.
        /// </summary>
        [ValidateNotNullOrEmpty]
        [Parameter(Mandatory = true, HelpMessage = "The name of the person to greet")]
        public string Name { get; set; }

        /// <summary>
        ///     The title of the person to greet, without a period after it.
        /// </summary>
        [Parameter(ValueFromRemainingArguments = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Title of the person to greet, sans period.")]
        public string Title { get; set; }

        /// <summary>
        ///     Perform Cmdlet processing.
        /// </summary>
        protected override void ProcessRecord()
        {
            if(string.IsNullOrEmpty(Title))
                WriteObject($"Hello, {Name}!");
            else
                WriteObject($"Hello, {Title} {Name}!");
        }
    }
}
