using System;
using Xunit;

namespace PSReptile.Tests
{
    /// <summary>
    ///     Tests for PSReptile attributes.
    /// </summary>
    public sealed class AttributeTests
    {
        [Fact]
        public void New_CmdletDescriptionAttribute_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => {
                new CmdletDescriptionAttribute(null);
            });
        }

        [Fact]
        public void New_CmdletDescriptionAttribute_NoException()
        {
            try {
                new CmdletDescriptionAttribute(string.Empty);
            } catch(ArgumentNullException) {
                Assert.True(false, "Threw an ArgumentNullException when given string.Empty");
            }
            Assert.True(true);
        }

        [Fact]
        public void New_CmdletSynopsisAttribute_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => {
                new CmdletSynopsisAttribute(null);
            });
        }

        [Fact]
        public void New_CmdletSynopsisAttribute_NoException()
        {
            try {
                new CmdletSynopsisAttribute(string.Empty);
            } catch(ArgumentNullException) {
                Assert.True(false, "Threw an ArgumentNullException when given string.Empty");
            }
        }

        [Theory]
        [InlineData("title", null, "", "")]
        [InlineData("code", "", null, "")]
        [InlineData("remarks", "", "", null)]
        public void New_CmdletExampleAttribute_ThrowsArgumentNullException(string paramName, string title, string code, string remarks)
        {
            Assert.Throws<ArgumentNullException>(paramName, () => {
                new CmdletExampleAttribute(title: title,  code: code, remarks: remarks);
            });
        }

        [Fact]
        public void New_CmdletExampleAttribute_NoException()
        {
            try {
                new CmdletExampleAttribute(title: string.Empty,  code: string.Empty, remarks: string.Empty);
            } catch(ArgumentNullException e) {
                Assert.True(false, $"Threw an ArgumentNullException when given string.Empty for parameter {e.ParamName}");
            }
        }

        [Fact]
        public void New_CmdletInputTypeAttribute_AcceptsCLRType()
        {
            var attribute = new CmdletInputTypeAttribute(typeof(string));
            Assert.True(attribute.IsCLRType);
            Assert.Equal(typeof(string).Name, attribute.Name);
        }

        [Fact]
        public void New_CmdletInputTypeAttribute_AcceptsNullCLRType()
        {
            var attribute = new CmdletInputTypeAttribute((Type) null);
            Assert.False(attribute.IsCLRType);
            Assert.Equal("None", attribute.Name);
        }

        [Theory]
        [InlineData("string", "string")]
        [InlineData(null, "None")]
        [InlineData("", "None")]
        [InlineData(" ", " ")]
        public void New_CmdletInputTypeAttribute_String(string parameter, string expected)
        {
            var attribute = new CmdletInputTypeAttribute(parameter);
            Assert.False(attribute.IsCLRType);
            Assert.Null(attribute.Type);
            Assert.Equal(expected, attribute.Name);
        }
    }
}
