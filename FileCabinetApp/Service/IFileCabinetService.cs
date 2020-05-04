using System.Collections.Generic;

namespace FileCabinetApp
{
    /// <summary>
    /// File cabinet service interface.
    /// </summary>
    public interface IFileCabinetService
    {
        int CreateAndSetId(ValidateParametersData data);

        public int CreateRecord(int id, ValidateParametersData data);

        void EditRecord(int id, ValidateParametersData data);

        IEnumerable<FileCabinetRecord> GetRecords();

        FileCabinetServiceSnapshot MakeSnapshot();

        public void Restore(FileCabinetServiceSnapshot snapshot);

        void RemoveRecord(int id);

        public void Delete(IEnumerable<FileCabinetRecord> records);

        public void Purge();

        public (int active, int removed) GetStat();
    }
}
