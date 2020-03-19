using System;
using System.Collections.ObjectModel;
using System.Text;

namespace FileCabinetApp
{
    public interface IFileCabinetService
    {
        int CreateRecord((string firstName, string lastName, DateTime dateOfBirth, short bonuses, decimal salary, char accountType) data);

        void EditRecord(int id, (string firstName, string lastName, DateTime dateOfBirth, short bonuses, decimal salary, char accountType) data);

        ReadOnlyCollection<FileCabinetRecord> GetRecords();

        ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName);

        ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName);

        ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(DateTime dateOfBirth);

        FileCabinetServiceSnapshot MakeSnapshot();

        int GetStat();

        public void Restore(FileCabinetServiceSnapshot snapshot, out int failed);
    }
}
