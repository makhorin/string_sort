using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using StringComparer = StringSorting.Common.StringComparer;

namespace StringSorting.Test
{
    [TestFixture]
    public class ComparerTest
    {
        [TestCase("1. Apple", "2. Apple", -1)]
        [TestCase("2. Apple", "1. Apple", 1)]
        [TestCase("2. Apple", "1. Banana", -1)]
        [TestCase("1. Apple", "1. Apple", 0)]
        [TestCase("999. A Very Long String", "0. Banana", -1)]
        [TestCase("30432. Something something something", "415. Apple", 1)]
        [TestCase("1. Apple", "30432. Something something something", -1)]
        [TestCase("1. Apple", "1. Apples", -1)]
        public void ItShouldCompare(string x, string y, int result)
        {
            var comparer = new StringComparer();
            Assert.AreEqual(Math.Sign(result), Math.Sign(comparer.Compare(x, y)));
        }

        [Test]
        public void ItShouldSortUsingComparer()
        {
            var comparer = new StringComparer();
            var lst = File.ReadAllLines("TestCase.txt").ToList();
            var expected = File.ReadAllLines("ExpectedResult.txt");
            lst.Sort(comparer);
            Assert.IsTrue(lst.SequenceEqual(expected));
        }
		
		[TestCase("1, Apple", "30432. Something something something")]
        public void ItShouldThrowOnIncorrectInput(string x, string y)
        {
            var comparer = new StringComparer();
            Assert.Throws<ArgumentException>(() => comparer.Compare(x, y));
        }
    }
}