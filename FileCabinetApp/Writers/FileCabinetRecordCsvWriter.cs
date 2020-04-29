using System.IO;
using System;

namespace FileCabinetApp
{
    public class FileCabinetRecordCsvWriter : IFileCabinetWriter
    {
        private readonly StreamWriter writer;

        public FileCabinetRecordCsvWriter(StreamWriter writer)
        {
            this.writer = writer ?? throw new ArgumentNullException(nameof(writer));
        }

        public void Write(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            this.writer.WriteLine(record.ToString());
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
