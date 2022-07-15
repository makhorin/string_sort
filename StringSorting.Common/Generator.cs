using System;
using System.IO;

namespace StringSorting.Common
{
    public class Generator
    {
        private readonly int _bufferSize;

        public Generator(int bufferSize)
        {
            _bufferSize = bufferSize;
        }

        public void Generate(ulong bytesToGenerate, string outputFile)
        {
            var rnd  = new Random();
            var currentBuffer = 0;
            var iteration = 0;
            using var writer = File.AppendText(outputFile);
            while (bytesToGenerate > 0)
            {
                var strLen = rnd.Next(10, 1024);
                var bytes = new byte[Math.Max(1, strLen / 4 * 3)];
                rnd.NextBytes(bytes);
                var line = Convert.ToBase64String(bytes)
                    .Replace("=","")
                    .Replace("+","")
                    .Replace("/","");

                var i = 0;
                for (; i <= iteration % 2; i++)
                {
                    writer.WriteLine($"{rnd.Next(10000)}. {line}");
                }

                strLen *= i;

                if (bytesToGenerate < (ulong)strLen) bytesToGenerate = 0;
                else bytesToGenerate -= (ulong)strLen;
                
                currentBuffer += strLen;
                if (currentBuffer >= _bufferSize)
                {
                    writer.Flush();
                    currentBuffer = 0;
                }

                iteration++;
            }
            writer.Flush();
        }
    }
}
