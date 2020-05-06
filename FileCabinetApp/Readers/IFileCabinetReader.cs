using System;
using System.Collections.Generic;

namespace FileCabinetApp
{
    /// <summary>
    /// File cabiner reader.
    /// </summary>
    public interface IFileCabinetReader : IDisposable
    {
        /// <summary>
        /// Read all in file.
        /// </summary>
        /// <returns>Records.</returns>
        IList<FileCabinetRecord> ReadAll();
    }
}
