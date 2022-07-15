using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using StringSorting.Common;

namespace StringSorting.Test
{

    [TestFixture]
    public class SorterTest
    {
        [Test]
        public async Task ItShouldSort()
        {
            var sorter = new Sorter(1024 * 1024);
            await sorter.SortAsync("TestCase.txt", "sorted");
            var sorted = File.ReadAllLines("sorted");
            var expected = File.ReadAllLines("ExpectedResult.txt");
            Assert.IsTrue(expected.SequenceEqual(sorted));
        }
    }
}
