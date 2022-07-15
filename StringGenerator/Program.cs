using System;
using StringSorting.Common;

namespace StringGenerator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Specify number of bytes to generate in first argument");
                Console.ReadKey();
                return;
            }

            var outputFile = "test_file_gen";
            if (args.Length > 1)
            {
                outputFile = args[1];
            }

            var bytesToGenerate = 0UL;
            try
            {
                bytesToGenerate = Convert.ToUInt64(args[0]);
            }
            catch
            {
                Console.WriteLine("Number of bytes must be non-negative integer");
            }

            if (bytesToGenerate == 0)
            {
                Console.WriteLine("Nothing to do");
                return;
            }

            var generator = new Generator((int)Math.Pow(1024,3));
            generator.Generate(bytesToGenerate, outputFile);
        }
    }
}
