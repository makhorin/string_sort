using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace StringSorting.Common
{
    public class Sorter
    {
        private readonly int _bufferSize;
        private static readonly StringComparer Comparer = new StringComparer();
        private static readonly int EffectiveThreads = Environment.ProcessorCount * 2;
        private int _fileNumber;

        public Sorter(int bufferSize)
        {
            _bufferSize = bufferSize;
        }

        public async Task SortAsync(string inputFile, string outputFile)
        {
            _fileNumber = 0;
            var toSort = new List<string>[EffectiveThreads];

            for (var i = 0; i < toSort.Length; i++)
            {
                toSort[i] = new List<string>();
            }

            var fileNum = 0;
            var currentChunk = 0;
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            Console.WriteLine("Start reading and sorting chunks...");
            var sortedChunks = 0;
            foreach (var line in File.ReadLines(inputFile))
            {
                toSort[fileNum].Add(line);
                fileNum++;
                if (fileNum == toSort.Length) fileNum = 0;
                currentChunk += line.Length;

                if (currentChunk > _bufferSize)
                {
                    await SortIt(toSort);
                    currentChunk = 0;
                    sortedChunks += toSort.Length;
                    Console.Write("\r{0} chunks sorted", sortedChunks);
                }
            }
            await SortIt(toSort);
            sortedChunks += toSort.Length;
            Console.WriteLine("\r{0} chunks sorted", sortedChunks);
            Console.WriteLine($"It took {stopWatch.ElapsedMilliseconds}ms to sort chunks");
            var readers = new StreamReader[_fileNumber];
            var lines = new string[_fileNumber];
            for (var i = 0; i < _fileNumber; i++)
            {
                var stream = File.OpenRead($"partly_sorted_{i}");
                readers[i] = new StreamReader(stream);
                lines[i] = readers[i].ReadLine();
            }
            Console.WriteLine("Start merging...");
            using var fs = File.OpenWrite(outputFile);
            using var sw = new StreamWriter(fs);
            while (true)
            {
                var minIdx = 0;
                for (var i = 1; i < lines.Length; i++)
                {
                    if (string.IsNullOrEmpty(lines[minIdx]))
                    {
                        minIdx = i;
                        continue;
                    }

                    if (string.IsNullOrEmpty(lines[i])) continue;
                    if (Comparer.Compare(lines[i], lines[minIdx]) < 0)
                    {
                        minIdx = i;
                    }
                }

                var min = lines[minIdx];
                if (min == null) break;

                if (readers[minIdx].EndOfStream) lines[minIdx] = null;
                else lines[minIdx] = readers[minIdx].ReadLine();
                sw.WriteLine(min);
            }

            sw.Flush();
            sw.Close();
            stopWatch.Stop();
                
            Console.WriteLine($"It took {stopWatch.ElapsedMilliseconds}ms to sort and write everything");
            stopWatch.Restart();
            foreach (var r in readers)
            {
                r.Close();
            }
            for (var i = 0; i < _fileNumber; i++)
            {
                File.Delete($"partly_sorted_{i}");
            }
            Console.WriteLine($"It took another {stopWatch.ElapsedMilliseconds}ms to cleanup");
        }

        private async Task SortIt(List<string>[] toSort)
        {
            var sortTasks = new Task[EffectiveThreads];
            for (var j = 0; j < toSort.Length; j++)
            {
                if (toSort[j].Count == 0)
                {
                    sortTasks[j] = Task.CompletedTask;
                }
                else
                {
                    var indexToSort = j;
                    var fileNumber = _fileNumber;
                    sortTasks[j] = Task.Run(() => SortAndFlush(toSort[indexToSort], fileNumber)
                        .ContinueWith(t => toSort[indexToSort] = new List<string>()));
                    _fileNumber++;
                }
            }

            await Task.WhenAll(sortTasks).ConfigureAwait(false);
        }

        private async Task SortAndFlush(List<string> toSort, int fileNumber)
        {
            toSort.Sort(Comparer);
            await File.WriteAllLinesAsync($"partly_sorted_{fileNumber}", toSort).ConfigureAwait(false);
        }
    }
}
