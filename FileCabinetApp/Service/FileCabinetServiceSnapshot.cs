using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FileCabinetApp
{
    /// <summary>
    /// Snapshot of file cabinet service.
    /// </summary>
    public class FileCabinetServiceSnapshot
    {
        private FileCabinetRecord[] records;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetServiceSnapshot"/> class.
        /// </summary>
        /// <param name="records">Records.</param>
        public FileCabinetServiceSnapshot(FileCabinetRecord[] records)
        {
            this.records = records;
        }

        /// <summary>
        /// Gets logger.
        /// </summary>
        /// <value>
        /// Logger.
        /// </value>
        public IList<string> Logger { get; } = new List<string>();

        /// <summary>
        /// Gets all records.
        /// </summary>
        /// <value>
        /// All records.
        /// </value>
        public ReadOnlyCollection<FileCabinetRecord> Records
        {
            get => new ReadOnlyCollection<FileCabinetRecord>(new List<FileCabinetRecord>(this.records));
        }

        /// <summary>
        /// Load from file.
        /// </summary>
        /// <param name="reader">Reader.</param>
        public void LoadFrom(IFileCabinetReader reader)
        {
            if (reader is null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            try
            {
                this.records = reader.ReadAll().ToArray();
            }
            catch (Exception)
            {
                throw new ArgumentException("Cannot load file.");
            }
        }

        /// <summary>
        /// Save to file.
        /// </summary>
        /// <param name="writer">Writer.</param>
        public void SaveTo(IFileCabinetWriter writer)
        {
            if (writer is null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            foreach (var record in this.records)
            {
                writer.Write(record);
            }
        }
    }
}
