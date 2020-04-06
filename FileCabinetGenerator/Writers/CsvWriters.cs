using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace FileCabinetGenerator
{
    public class CsvWriter
    {
        StreamWriter fileStream;
        IEnumerable<FileCabinetRecordSerializable> records;

        public CsvWriter(StreamWriter fileStream, IEnumerable<FileCabinetRecordSerializable> records)
        {
            this.fileStream = fileStream;
            this.records = records;
        }

        public void Write()
        {
            this.fileStream.WriteLine("Id,First Name,Last Name,Date of Birth,AccountType,Salary,Bonuses");
            foreach (var record in this.records)
            {
                var result = new StringBuilder();
                result.AppendLine(string.Format(CultureInfo.InvariantCulture,
                    $"{record.Id}," +
                    $"{record.Name.FirstName}," +
                    $"{record.Name.LastName}," +
                    $"{record.DateOfBirth.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)}," +
                    $"{record.AccountType}," +
                    $"{record.Salary}," +
                    $"{record.Bonuses}."));

                this.fileStream.Write(result.ToString());
            }
        }
    }
}
