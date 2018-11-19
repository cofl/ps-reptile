using System;
using System.IO;
using System.Xml.Serialization;
using Xunit;
using Xunit.Abstractions;

namespace PSReptile.Tests
{
    using Maml;

    /// <summary>
    ///     Tests for MAML XML generation.
    /// </summary>
    public class XmlTests
    {
        /// <summary>
        ///     Create a new XML generation test suite.
        /// </summary>
        /// <param name="output">
        ///     Xunit test output.
        /// </param>
        public XmlTests(ITestOutputHelper output)
        {
            if (output == null)
                throw new ArgumentNullException(nameof(output));

            Output = output;
        }

        /// <summary>
        ///     Xunit test output.
        /// </summary>
        ITestOutputHelper Output { get; }

        /// <summary>
        ///     Generate MAML XML from a <see cref="Cmdlet"/>.
        /// </summary>
        [Fact]
        public void GenerateXml_From_Cmdlet()
        {
            string expected = @"
<?xml version=""1.0"" encoding=""utf-16""?>
<command:command xmlns:maml=""http://schemas.microsoft.com/maml/2004/10"" xmlns:dev=""http://schemas.microsoft.com/maml/dev/2004/10"" xmlns:command=""http://schemas.microsoft.com/maml/dev/command/2004/10"">
  <command:details>
    <command:name>Get-Greeting</command:name>
    <maml:description>
      <maml:para>A simple Cmdlet that outputs a greeting to the pipeline</maml:para>
    </maml:description>
    <command:verb>Get</command:verb>
    <command:noun>Greeting</command:noun>
  </command:details>
  <maml:description>
    <maml:para>This Cmdlet works with greetings.</maml:para>
    <maml:para>It gets them.</maml:para>
    <maml:para>I can't see how to make it any clearer than that.</maml:para>
  </maml:description>
  <command:syntax>
    <command:syntaxItem>
      <maml:name>Get-Greeting</maml:name>
      <command:parameter required=""true"" variableLength=""true"" globbing=""false"" pipelineInput=""false"" position=""named"" aliases=""none"">
        <maml:name>Name</maml:name>
        <maml:description>
          <maml:para>The name of the person to greet</maml:para>
        </maml:description>
        <command:parameterValue required=""true"" variableLength=""false"">String</command:parameterValue>
        <dev:defaultValue>None</dev:defaultValue>
      </command:parameter>
      <command:parameter required=""false"" variableLength=""true"" globbing=""false"" pipelineInput=""true (ByPropertyName, FromRemainingArguments)"" position=""named"" aliases=""Honorific"">
        <maml:name>Title</maml:name>
        <maml:description>
          <maml:para>Title of the person to greet, sans period.</maml:para>
        </maml:description>
        <command:parameterValue required=""true"" variableLength=""false"">String</command:parameterValue>
        <dev:defaultValue>None</dev:defaultValue>
      </command:parameter>
      <command:parameter required=""true"" variableLength=""true"" globbing=""false"" pipelineInput=""true (ByValue)"" position=""named"" aliases=""none"">
        <maml:name>Greeting</maml:name>
        <maml:description>
          <maml:para>The last greeting to use.</maml:para>
        </maml:description>
        <command:parameterValue required=""true"" variableLength=""false"">String</command:parameterValue>
        <dev:defaultValue>Hello</dev:defaultValue>
      </command:parameter>
    </command:syntaxItem>
    <command:syntaxItem>
      <maml:name>Get-Greeting</maml:name>
      <command:parameter required=""true"" variableLength=""true"" globbing=""false"" pipelineInput=""false"" position=""named"" aliases=""none"">
        <maml:name>Name</maml:name>
        <maml:description>
          <maml:para>The name of the person to greet</maml:para>
        </maml:description>
        <command:parameterValue required=""true"" variableLength=""false"">String</command:parameterValue>
        <dev:defaultValue>None</dev:defaultValue>
      </command:parameter>
      <command:parameter required=""false"" variableLength=""true"" globbing=""false"" pipelineInput=""true (ByPropertyName, FromRemainingArguments)"" position=""named"" aliases=""Honorific"">
        <maml:name>Title</maml:name>
        <maml:description>
          <maml:para>Title of the person to greet, sans period.</maml:para>
        </maml:description>
        <command:parameterValue required=""true"" variableLength=""false"">String</command:parameterValue>
        <dev:defaultValue>None</dev:defaultValue>
      </command:parameter>
      <command:parameter required=""true"" variableLength=""true"" globbing=""false"" pipelineInput=""true (ByValue)"" position=""named"" aliases=""none"">
        <maml:name>Greeting</maml:name>
        <maml:description>
          <maml:para>The last greeting to use.</maml:para>
        </maml:description>
        <command:parameterValue required=""true"" variableLength=""false"">String</command:parameterValue>
        <dev:defaultValue>Hello</dev:defaultValue>
      </command:parameter>
    </command:syntaxItem>
  </command:syntax>
  <command:parameters>
    <command:parameter required=""true"" variableLength=""true"" globbing=""false"" pipelineInput=""false"" position=""named"" aliases=""none"">
      <maml:name>Name</maml:name>
      <maml:description>
        <maml:para>The name of the person to greet</maml:para>
      </maml:description>
      <command:parameterValue required=""true"" variableLength=""false"">String</command:parameterValue>
      <dev:defaultValue>None</dev:defaultValue>
    </command:parameter>
    <command:parameter required=""false"" variableLength=""true"" globbing=""false"" pipelineInput=""true (ByPropertyName, FromRemainingArguments)"" position=""named"" aliases=""Honorific"">
      <maml:name>Title</maml:name>
      <maml:description>
        <maml:para>Title of the person to greet, sans period.</maml:para>
      </maml:description>
      <command:parameterValue required=""true"" variableLength=""false"">String</command:parameterValue>
      <dev:defaultValue>None</dev:defaultValue>
    </command:parameter>
    <command:parameter required=""true"" variableLength=""true"" globbing=""false"" pipelineInput=""true (ByValue)"" position=""named"" aliases=""none"">
      <maml:name>Greeting</maml:name>
      <maml:description>
        <maml:para>The last greeting to use.</maml:para>
      </maml:description>
      <command:parameterValue required=""true"" variableLength=""false"">String</command:parameterValue>
      <dev:defaultValue>Hello</dev:defaultValue>
    </command:parameter>
  </command:parameters>
  <command:inputTypes />
  <command:returnValues />
  <command:examples />
</command:command>
            ".Trim();

            Command cmdletHelp = new MamlGenerator().Generate(
                typeof(SampleModule.GetGreeting)
            );

            XmlSerializerNamespaces namespacesWithPrefix = Constants.XmlNamespace.GetStandardPrefixes();
            XmlSerializer serializer = new XmlSerializer(typeof(Command));

            string actual;
            using (StringWriter writer = new StringWriter { NewLine = "\n" })
            {
                serializer.Serialize(writer, cmdletHelp, namespaces: namespacesWithPrefix);
                writer.Flush();

                actual = writer.ToString();
            }
            using(var k = new StreamWriter(@"C:\BHK\out.txt"))
            {
                k.Write(actual);
                k.Flush();
            }
            
            Assert.Equal(expected, actual);
        }

        /// <summary>
        ///     Generate MAML XML from a <see cref="Command"/>.
        /// </summary>
        [Fact]
        public void GenerateXml_From_Command()
        {
            string expected = @"
<?xml version=""1.0"" encoding=""utf-16""?>
<command:command xmlns:maml=""http://schemas.microsoft.com/maml/2004/10"" xmlns:dev=""http://schemas.microsoft.com/maml/dev/2004/10"" xmlns:command=""http://schemas.microsoft.com/maml/dev/command/2004/10"">
  <command:details>
    <command:name>Get-FooBar</command:name>
    <maml:description>
      <maml:para>Retrieve one or more FooBars.</maml:para>
    </maml:description>
    <command:verb>Get</command:verb>
    <command:noun>FooBar</command:noun>
  </command:details>
  <maml:description>
    <maml:para>This command works with FooBars.</maml:para>
    <maml:para>It gets them.</maml:para>
    <maml:para>I don't really know how to make it any clearer than that.</maml:para>
  </maml:description>
  <command:syntax>
    <command:syntaxItem>
      <maml:name>Get-FooBar</maml:name>
      <command:parameter required=""true"" variableLength=""true"" globbing=""false"" pipelineInput=""true (ByValue)"" position=""0"" aliases=""none"">
        <maml:name>Name</maml:name>
        <maml:description>
          <maml:para>The bar name</maml:para>
        </maml:description>
        <command:parameterValue required=""true"" variableLength=""false"">String</command:parameterValue>
        <dev:defaultValue>None</dev:defaultValue>
      </command:parameter>
    </command:syntaxItem>
  </command:syntax>
  <command:parameters>
    <command:parameter required=""true"" variableLength=""true"" globbing=""false"" pipelineInput=""true (ByValue)"" position=""0"" aliases=""none"">
      <maml:name>Name</maml:name>
      <maml:description>
        <maml:para>The bar name</maml:para>
      </maml:description>
      <command:parameterValue required=""true"" variableLength=""false"">String</command:parameterValue>
      <dev:defaultValue>None</dev:defaultValue>
    </command:parameter>
  </command:parameters>
  <command:inputTypes />
  <command:returnValues />
  <command:examples />
</command:command>
            ".Trim();

            XmlSerializerNamespaces namespacesWithPrefix = Constants.XmlNamespace.GetStandardPrefixes();
            XmlSerializer serializer = new XmlSerializer(typeof(Command));

            string actual;
            using (StringWriter writer = new StringWriter { NewLine = "\n" })
            {
                Parameter nameParameter = new Parameter
                {
                    Position = "0",
                    Name = "Name",
                    Description =
                    {
                        "The bar name"
                    },
                    Value =
                    {
                        DataType = "String",
                        IsMandatory = true
                    },
                    IsMandatory = true,
                    SupportsPipelineInput = PipelineInputType.ByValue
                };

                Command getFooBarCommand = new Command
                {
                    Details =
                    {
                        Name = "Get-FooBar",
                        Verb = "Get",
                        Noun = "FooBar",
                        Synopsis =
                        {
                            "Retrieve one or more FooBars."
                        }
                    },
                    Description =
                    {
                        "This command works with FooBars.",
                        "It gets them.",
                        "I don't really know how to make it any clearer than that."
                    },
                    Parameters =
                    {
                        nameParameter
                    },
                    Syntax =
                    {
                        new SyntaxItem
                        {
                            CommandName = "Get-FooBar",
                            Parameters =
                            {
                                nameParameter
                            },
                        }
                    }       
                };

                serializer.Serialize(writer, getFooBarCommand, namespaces: namespacesWithPrefix);

                writer.Flush();

                actual = writer.ToString();
            }

            Assert.Equal(expected, actual);
        }
    }
}
