using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace FileCabinetApp
{
    public class FileCabinetRecordCsvWriter
    {
        private TextWriter textWriter;

        public FileCabinetRecordCsvWriter(TextWriter textWriter) => this.textWriter = textWriter;

        public void Write(FileCabinetRecord record)
        {
            this.textWriter.Write(record.ToString());
        }
    }
}
