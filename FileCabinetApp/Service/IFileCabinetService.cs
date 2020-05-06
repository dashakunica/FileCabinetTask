﻿using System.Collections.Generic;

namespace FileCabinetApp
{
    /// <summary>
    /// File cabinet service interface.
    /// </summary>
    public interface IFileCabinetService
    {
        /// <summary>
        /// Create record and set id.
        /// </summary>
        /// <param name="data">Data.</param>
        /// <returns>Id.</returns>
        int CreateAndSetId(ValidateParametersData data);

        /// <summary>
        /// Create record.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <param name="data">Data.</param>
        /// <returns>Identificator.</returns>
        public int CreateRecord(int id, ValidateParametersData data);

        /// <summary>
        /// Edir record.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <param name="data">Data.</param>
        void EditRecord(int id, ValidateParametersData data);

        /// <summary>
        /// Get all records.
        /// </summary>
        /// <returns>All records.</returns>
        IEnumerable<FileCabinetRecord> GetRecords();

        /// <summary>
        /// Make snapshot.
        /// </summary>
        /// <returns>Snapshot.</returns>
        FileCabinetServiceSnapshot MakeSnapshot();

        /// <summary>
        /// Restore snapshot.
        /// </summary>
        /// <param name="snapshot">Snapdhot.</param>
        public void Restore(FileCabinetServiceSnapshot snapshot);

        /// <summary>
        /// Remove record.
        /// </summary>
        /// <param name="id">Id.</param>
        void RemoveRecord(int id);

        /// <summary>
        /// Delete specific records.
        /// </summary>
        /// <param name="records">Records.</param>
        public void Delete(IEnumerable<FileCabinetRecord> records);

        /// <summary>
        /// Purge.
        /// </summary>
        public void Purge();

        /// <summary>
        /// Get statistic about active and removed records.
        /// </summary>
        /// <returns>Active and removed records.</returns>
        public (int active, int removed) GetStat();
    }
}
