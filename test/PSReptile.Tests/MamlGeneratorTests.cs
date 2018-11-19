using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Reflection;
using Xunit;

namespace PSReptile.Tests
{
    using Extractors;

    /// <summary>
    ///     Tests for <see cref="PSReptile.Reflector" />.
    /// </summary>
    public sealed class MamlGeneratorTests
    {
        private MamlGenerator NormalMamlGenerator = new MamlGenerator();

        [Fact]
        public void New_MamlGenerator_ThrowsArgumentNullException_Enumerable()
        {
            Assert.Throws<ArgumentNullException>(() => {
                new MamlGenerator((IEnumerable<IDocumentationExtractor>) null);
            });
        }

        [Fact]
        public void New_MamlGenerator_ThrowsArgumentNullException_Params()
        {
            Assert.Throws<ArgumentNullException>(() => {
                new MamlGenerator((IDocumentationExtractor) null);
            });
        }

        [Fact]
        public void Generate_ThrowsArgumentNullException_Assembly()
        {
            Assert.Throws<ArgumentNullException>(() => {
                NormalMamlGenerator.Generate((Assembly) null);
            });
        }

        [Fact]
        public void Generate_Assembly()
        {
            var assembly = Assembly.GetAssembly(typeof(SampleModule.GetGreeting));
            var help = NormalMamlGenerator.Generate(assembly);
            Assert.NotNull(help);
            Assert.NotEmpty(help.Commands);
        }

        [Fact]
        public void Generate_ThrowsArgumentNullException_Type()
        {
            Assert.Throws<ArgumentNullException>(() => {
                NormalMamlGenerator.Generate((Type) null);
            });
        }

        [Fact]
        public void Generate_ThrowsArgumentException_Type()
        {
            Assert.Throws<ArgumentException>(() => {
                // Using some known bad type, in this case an abstract cmdlet class.
                NormalMamlGenerator.Generate(typeof(ReflectorTestTypes.AbstractCmdlet));
            });
        }

        [Fact]
        public void Generate_NonParameterPropertyContinues()
        {
            var command = NormalMamlGenerator.Generate(typeof(MamlGeneratorTestTypes.NonParameterPropertyCmdlet));
            Assert.Empty(command.Parameters);
        }

        [Fact]
        public void Generate_FirstParameterWithNamedPosition()
        {
            var command = NormalMamlGenerator.Generate(typeof(MamlGeneratorTestTypes.DefaultValueCmdlet));
            Assert.Equal("named", command.Parameters[0].Position);
        }

        [Fact]
        public void Generate_FirstParameterWithIntegerPosition()
        {
            var command = NormalMamlGenerator.Generate(typeof(MamlGeneratorTestTypes.FirstParameterWithPositionCmdlet));
            Assert.Equal("0", command.Parameters[0].Position);
        }

        [Fact]
        public void Generate_DefaultValue()
        {
            var command = NormalMamlGenerator.Generate(typeof(MamlGeneratorTestTypes.DefaultValueCmdlet));
            Assert.Equal("Hello!", command.Parameters[0].DefaultValue);
        }
    }

    namespace MamlGeneratorTestTypes
    {
        [Cmdlet("Verb", "Noun")]
        public class NonParameterPropertyCmdlet: Cmdlet
        {
            public string NonParameterProperty { get; set; }
        }

        [Cmdlet("Verb", "Noun")]
        public class FirstParameterWithPositionCmdlet: Cmdlet
        {
            [Parameter(Position = 0)]
            public string ParameterProperty { get; set; }
        }

        [Cmdlet("Verb", "Noun")]
        public class DefaultValueCmdlet: Cmdlet
        {
            [Parameter]
            public string ParameterProperty { get; set; } = "Hello!";
        }
    }
}
