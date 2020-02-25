using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace FileCabinetApp
{
    public class FileCabinetServiceSnapshot
    {
        private FileCabinetRecord[] records;

        public FileCabinetServiceSnapshot(FileCabinetRecord[] record) => this.records = records;

        //public FileCabinetRecord[] Record
        //{
        //    get
        //    {
        //        if (this.record is null)
        //        {
        //            throw new ArgumentNullException(nameof(record));
        //        }

        //        return record;
        //    }
        //    set => this.record = value;
        //}

        //public FileCabinetRecord[] GetRecords()
        //{
        //    return (FileCabinetRecord[])this.record.Clone();
        //}

        public void SaveToCsv(StreamWriter streamWriter)
        {
            TextWriter textWriter = streamWriter;

            textWriter.Write("Id,First Name,Last Name,Date of Birth");
            FileCabinetRecordCsvWriter fileCabinetRecordCsvWriter = new FileCabinetRecordCsvWriter(textWriter);

            foreach (var record in this.records)
            {
                fileCabinetRecordCsvWriter.Write(record);
            }
        }
    }
}
