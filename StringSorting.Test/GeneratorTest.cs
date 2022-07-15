using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using StringSorting.Common;

namespace StringSorting.Test
{
    [TestFixture]
    public class GeneratorTest
    {
        [TestCase(1)]
        [TestCase(1024)]
        [TestCase(1024*1024*1024)]
        public void ItShouldGenerateFileOfSpecifiedSize(int expected)
        {
            File.Delete("test_gen");
            var generator = new Generator(1024);
            generator.Generate((ulong)expected, "test_gen");
            var bytes = File.ReadAllBytes("test_gen");
            Assert.IsTrue(Math.Abs(expected - bytes.Length) < Math.Max(1024,expected * 0.1f));
            Assert.IsTrue(bytes.Length > 0);
        }

        [Test]
        public void ItShouldGenerateFileWithDuplicates()
        {
            File.Delete("test_gen");
            var generator = new Generator(1024);
            generator.Generate((ulong)1024*1024, "test_gen");
            var regEx = new Regex("^([0-9]+)\\.\\s([\\w]+)$");
            var strings = File.ReadAllLines("test_gen").Select(l => regEx.Match(l).Groups[2].Value).ToArray();
            var total = strings.Length;
            var distinct = strings.Distinct().Count();
            Assert.AreNotEqual(total, distinct);
        }
    }
}
