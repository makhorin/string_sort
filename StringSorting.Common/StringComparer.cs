using System;
using System.Collections.Generic;

namespace StringSorting.Common
{
    public class StringComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            try
            {
                var byNum = 0;
                var byStr = 0;
                var i = 0;
                var j = 0;
                while (byNum == 0 && x[i] != '.' && y[i] != '.')
                {
                    byNum = x[i].CompareTo(y[j]);
                    i++;
                    j++;
                }

                while (x[i] != '.') i++;
                while (y[j] != '.') j++;

                byNum = i == j ? byNum : i.CompareTo(j);
                i += 2;
                j += 2;
                while (i < x.Length && j < y.Length)
                {
                    var result = x[i].CompareTo(y[j]);
                    if (result != 0)
                    {
                        byStr = result;
                        break;
                    }

                    i++;
                    j++;
                }

                return byStr == 0 ? byNum : byStr;
            }
            catch
            {
                Console.WriteLine("Looks like file has incorrect format");
                throw;
            }
        }
    }
}
