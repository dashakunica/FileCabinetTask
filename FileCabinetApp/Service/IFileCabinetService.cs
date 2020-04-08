using System;
using System.Collections.Generic;

namespace FileCabinetApp
{
    public interface IFileCabinetService
    {
        int CreateRecord((string firstName, string lastName, DateTime dateOfBirth, short bonuses, decimal salary, char accountType) data);

        void EditRecord(int id, (string firstName, string lastName, DateTime dateOfBirth, short bonuses, decimal salary, char accountType) data);

        IEnumerable<FileCabinetRecord> GetRecords();

        IEnumerable<FileCabinetRecord> FindByFirstName(string firstName);

        IEnumerable<FileCabinetRecord> FindByLastName(string lastName);

        IEnumerable<FileCabinetRecord> FindByDateOfBirth(DateTime dateOfBirth);

        FileCabinetServiceSnapshot MakeSnapshot();

        public void Restore(FileCabinetServiceSnapshot snapshot);

        void RemoveRecord(int id);

        public void Purge();

        public (int active, int removed) GetStat();
    }
}
