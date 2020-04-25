using System;
using System.Collections.Generic;

namespace FileCabinetApp
{
    public interface IFileCabinetService
    {
        int CreateRecord(ValidateParametersData data);

        public int CreateRecordWithId(int id, ValidateParametersData data);

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
