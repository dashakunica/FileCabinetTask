using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    public static class Memoization
    {
        public static readonly List<Tuple<string, IEnumerable<FileCabinetRecord>>> Saved = new List<Tuple<string, IEnumerable<FileCabinetRecord>>>();

        public static void RefreshMemoization()
        {
            Saved.Clear();
        }
    }
}
