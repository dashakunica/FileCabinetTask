using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.Iterators
{
    public class MemoryIterator : IEnumerator<FileCabinetRecord>, IEnumerable<FileCabinetRecord>
    {
        private readonly List<FileCabinetRecord> list;
        private int position = -1;
        private bool disposed = false;

        public MemoryIterator(List<FileCabinetRecord> list)
        {
            this.list = list ?? new List<FileCabinetRecord>();
        }

        public FileCabinetRecord Current => this.list[this.position];

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
