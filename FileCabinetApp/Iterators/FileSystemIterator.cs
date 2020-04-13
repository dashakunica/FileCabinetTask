using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace FileCabinetApp.Iterators
{
    public class FileSystemIterator : IEnumerator<FileCabinetRecord>, IEnumerable<FileCabinetRecord>
    {
        private const int Zero = 0;
        private const int RecordSize = 278;
        private const int UnicodeSize = 2;
        private const int MaxStringValue = 60;
        private const char WhiteSpace = ' ';

        private readonly BinaryReader reader;
        private readonly List<long> list;
        private int position = -1;
        private bool disposed = false;

        public FileSystemIterator(List<long> list, BinaryReader reader)
        {
            this.list = list ?? new List<long>();
            this.reader = reader ?? new BinaryReader(Stream.Null);
        }

        public FileCabinetRecord Current => this.ReadRecordFromFileStream(this.list[this.position], this.reader);

        object IEnumerator.Current => this.Current;

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public bool MoveNext()
        {
            if (this.list.Count == 0)
            {
                return false;
            }

            return ++this.position < this.list.Count ? true : false;
        }

        public void Reset()
        {
            this.position = -1;
        }

        public IEnumerator<FileCabinetRecord> GetEnumerator()
        {
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private FileCabinetRecord ReadRecordFromFileStream(long position, BinaryReader reader)
        {
            if (position % RecordSize != Zero)
            {
                throw new ArgumentException($"{position} isn't begin of the record.", nameof(position));
            }

            reader.BaseStream.Position = position;
            reader.ReadInt16();

            return new FileCabinetRecord()
            {
                Id = reader.ReadInt32(),
                FirstName = Encoding.Unicode.GetString(reader.ReadBytes(MaxStringValue * UnicodeSize)).Replace(char.MinValue, WhiteSpace).Trim(),
                LastName = Encoding.Unicode.GetString(reader.ReadBytes(MaxStringValue * UnicodeSize)).Replace(char.MinValue, WhiteSpace).Trim(),
                DateOfBirth = DateTime.Parse($"{reader.ReadInt32()}/{reader.ReadInt32()}/{reader.ReadInt32()}", CultureInfo.InvariantCulture),
                Bonuses = reader.ReadInt16(),
                Salary = reader.ReadDecimal(),
                AccountType = Encoding.Unicode.GetString(reader.ReadBytes(UnicodeSize)).First(),
            };
        }

        private void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.Reset();
            }

            this.disposed = true;
        }
    }
}
