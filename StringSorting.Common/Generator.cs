using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace StringSorting.Common
{
    public class Generator
    {
        private static readonly Regex StringFormat = new Regex("^([0-9]+)\\.\\s([\\w]+)$");
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
            while (bytesToGenerate >= 8)
            {
                var strLen = rnd.Next(10, 1024);
                var bytes = new byte[Math.Max(1, strLen / 4 * 3)];
                rnd.NextBytes(bytes);
                var line = Convert.ToBase64String(bytes)
                    .Replace("=","")
                    .Replace("+","")
                    .Replace("/","");

                var i = 0;
                for (; i <= iteration % 2 && bytesToGenerate >= 8; i++)
                {
                    var lineToWrite = $"{rnd.Next(10000)}. {line}";
                    if ((ulong)lineToWrite.Length * 2 > bytesToGenerate)
                    {
                        lineToWrite = new string(lineToWrite.Take((int)bytesToGenerate / 2).ToArray());
                        if (!StringFormat.IsMatch(lineToWrite))
                        {
                            lineToWrite = "1. ";
                            var addition = new string(Enumerable.Repeat('a', (int)(bytesToGenerate - 6UL) / 2).ToArray());
                            lineToWrite += addition;
                        }
                    }

                    bytesToGenerate -= (ulong)lineToWrite.Length;
                    writer.WriteLine(lineToWrite);
                }

                strLen *= i;
                
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
