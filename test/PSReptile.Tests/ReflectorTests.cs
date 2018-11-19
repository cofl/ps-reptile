using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Reflection;
using Xunit;

namespace PSReptile.Tests
{
    /// <summary>
    ///     Tests for <see cref="PSReptile.Reflector" />.
    /// </summary>
    public sealed class ReflectorTests
    {
        [Fact]
        public void IsCmdlet_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => {
                Reflector.IsCmdlet(null);
            });
        }

#region IsCmdlet Nested Types
        [Cmdlet("Verb", "Noun")]
        private class NestedPrivateCmdlet: Cmdlet {}

        [Cmdlet("Verb", "Noun")]
        public class NestedPublicCmdlet: Cmdlet {}
#endregion

        [Theory]
        [InlineData(typeof(NestedPrivateCmdlet), false)]
        [InlineData(typeof(NestedPublicCmdlet), false)]
        [InlineData(typeof(ReflectorTestTypes.InternalCmdlet), false)]
        [InlineData(typeof(ReflectorTestTypes.SomeStruct), false)]
        [InlineData(typeof(ReflectorTestTypes.SomeEnum), false)]
        [InlineData(typeof(ReflectorTestTypes.AbstractCmdlet), false)]
        [InlineData(typeof(ReflectorTestTypes.SealedCmdlet), true)]
        [InlineData(typeof(ReflectorTestTypes.DerivedCmdlet), true)]
        [InlineData(typeof(SampleModule.GetGreeting), true)]
        [InlineData(typeof(CmdletAttribute), false)]
        [InlineData(typeof(ReflectorTestTypes.MissingAttributeCmdlet), false)]
        public void IsCmdlet(Type type, bool expected)
        {
            Assert.Equal(expected, Reflector.IsCmdlet(type));
        }

        [Fact]
        public void IsCmdletParameter_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => {
                Reflector.IsCmdletParameter(null, out var _);
            });
        }

        [Fact]
        public void GetCmdletTypes_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => {
                // you need to force the coroutine to iterate for it to throw an exception.
                new List<Type>(Reflector.GetCmdletTypes(null));
            });
        }

        [Fact]
        public void GetCmdletTypes()
        {
            IEnumerable<Type> expected = new List<Type> { typeof(SampleModule.GetGreeting) };
            IEnumerable<Type> actual = Reflector.GetCmdletTypes(Assembly.GetAssembly(typeof(SampleModule.GetGreeting)));
            Assert.Equal(expected, actual);
        }
    }

    namespace ReflectorTestTypes
    {
        [Cmdlet("Verb", "Noun")]
        internal class InternalCmdlet: Cmdlet {}

        public struct SomeStruct {}

        public enum SomeEnum {}

        [Cmdlet("Verb", "Noun")]
        public abstract class AbstractCmdlet: Cmdlet {}

        [Cmdlet("Verb", "Noun")]
        public sealed class SealedCmdlet: Cmdlet {}

        [Cmdlet("Verb", "Noun")]
        public class DerivedCmdlet: AbstractCmdlet {}

        public class MissingAttributeCmdlet: Cmdlet {}
    }
}
