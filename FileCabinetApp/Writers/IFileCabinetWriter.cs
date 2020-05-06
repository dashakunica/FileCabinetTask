using System;

namespace FileCabinetApp
{
    /// <summary>
    /// File cabinet writer.
    /// </summary>
    public interface IFileCabinetWriter : IDisposable
    {
        /// <summary>
        /// Write in file.
        /// </summary>
        /// <param name="record">Record.</param>
        void Write(FileCabinetRecord record);
    }
}
