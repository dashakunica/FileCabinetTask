using System;
using System.Collections.ObjectModel;
using System.Text;

namespace FileCabinetApp
{
    public interface IFileCabinetService
    {
        int CreateRecord(FileCabinetRecord recordAddition);

        ReadOnlyCollection<FileCabinetRecord> GetRecords();

        void EditRecord(FileCabinetRecord record);

        ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName);

        ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName);

        ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(DateTime dateOfBirth);

        FileCabinetServiceSnapshot MakeSnapshot();

        int GetStat();
    }
}
