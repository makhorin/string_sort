using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using StringSorting.Common;

namespace StringSorting
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var fileName = args.Length > 0 ? args[0] : "test_file";
            if (!File.Exists(fileName))
            {
                Console.WriteLine($"File {fileName} not found. Specify file to sort in first argument");
                Console.ReadKey();
                return;
            }

            var sorter = new Sorter((int)Math.Pow(1024,3));
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            try
            {
                await sorter.SortAsync(fileName, "sorted");
                
            }
            catch (Exception e)
            {
                
                Console.WriteLine(e);
            }
            stopWatch.Stop();
            Console.WriteLine($"It took {stopWatch.ElapsedMilliseconds}ms to complete sorting");
        }
    }
}
