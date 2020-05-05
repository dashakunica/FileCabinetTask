using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Memoization.
    /// </summary>
    public static class Memoization
    {
        /// <summary>
        /// Saved fields.
        /// </summary>
        public static readonly List<Tuple<string, IEnumerable<FileCabinetRecord>>> Saved =
            new List<Tuple<string, IEnumerable<FileCabinetRecord>>>();

        /// <summary>
        /// Clear saved fields.
        /// </summary>
        public static void RefreshMemoization()
        {
            Saved.Clear();
        }
    }
}
