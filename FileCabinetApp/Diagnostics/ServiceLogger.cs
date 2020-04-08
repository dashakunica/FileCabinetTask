using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace FileCabinetApp
{
    public class ServiceLogger : IFileCabinetService, IDisposable
    {
        private readonly IFileCabinetService fileCabinetService;
        private readonly StreamWriter writer;
        private bool disposed = false;

        public ServiceLogger(IFileCabinetService fileCabinetService, string path)
        {
            this.fileCabinetService = fileCabinetService ?? throw new ArgumentNullException(nameof(fileCabinetService));

            if (path is null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            var stream = File.Exists(path) ? File.OpenWrite(path) : File.Create(path);
            this.writer = new StreamWriter(stream);
        }

        public string Name => nameof(ServiceLogger);

        public int CreateRecord((string firstName, string lastName, DateTime dateOfBirth, short bonuses, decimal salary, char accountType) data)
        {
            this.Print(nameof(this.CreateRecord), data.ToString());
            var value = this.fileCabinetService.CreateRecord(data);
            this.Print(nameof(this.CreateRecord), value.ToString(CultureInfo.InvariantCulture));
            return value;
        }

        public void EditRecord(int id, (string firstName, string lastName, DateTime dateOfBirth, short bonuses, decimal salary, char accountType) data)
        {
            this.Print(nameof(this.EditRecord), $"{nameof(id)} = '{id.ToString(CultureInfo.InvariantCulture)}', {data.ToString()}");
            this.fileCabinetService.EditRecord(id, data);
        }

        public IEnumerable<FileCabinetRecord> FindByDateOfBirth(DateTime dateOfBirth)
        {
            this.Print(nameof(this.FindByDateOfBirth), $"{nameof(dateOfBirth)} = '{dateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture)}'");
            var value = this.fileCabinetService.FindByDateOfBirth(dateOfBirth);
            this.Print(nameof(this.FindByDateOfBirth), $"{(value == null ? string.Empty : value.GetType().Name)}.");
            return value;
        }

        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
        {
            this.Print(nameof(this.FindByDateOfBirth), $"{nameof(firstName)} = '{firstName}'");
            var value = this.fileCabinetService.FindByFirstName(firstName);
            this.Print(nameof(this.FindByFirstName), $"{(value == null ? string.Empty : value.GetType().Name)}.");
            return value;
        }

        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName)
        {
            this.Print(nameof(this.FindByLastName), $"{nameof(lastName)} = '{lastName}'");
            var value = this.fileCabinetService.FindByLastName(lastName);
            this.Print(nameof(this.FindByLastName), $"{(value == null ? string.Empty : value.GetType().Name)}");
            return value;
        }

        public IEnumerable<FileCabinetRecord> GetRecords()
        {
            this.Print(nameof(this.GetRecords), string.Empty);
            var value = this.fileCabinetService.GetRecords();
            this.Print(nameof(this.GetRecords), $"{(value == null ? string.Empty : value.GetType().Name)}");
            return value;
        }

        public (int active, int removed) GetStat()
        {
            this.Print(nameof(this.GetStat), string.Empty);
            var value = this.fileCabinetService.GetStat();
            this.Print(nameof(this.GetStat), $"({value.active}, {value.removed})");
            return value;
        }

        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            this.Print(nameof(this.MakeSnapshot), string.Empty);
            var value = this.fileCabinetService.MakeSnapshot();
            this.Print(nameof(this.GetStat), $"snapshot with {value.Records.Count} elements.");
            return value;
        }

        public void Purge()
        {
            this.Print(nameof(this.Purge), string.Empty);
            this.fileCabinetService.Purge();
        }

        public void RemoveRecord(int id)
        {
            this.Print(nameof(this.RemoveRecord), $"{nameof(id)} = '{id}'");
            this.fileCabinetService.RemoveRecord(id);
        }

        public void Restore(FileCabinetServiceSnapshot snapshot)
        {
            if (snapshot is null)
            {
                throw new ArgumentNullException(nameof(snapshot));
            }

            this.Print(nameof(this.Restore), $"snapshot wich contains {snapshot?.Records.Count} elemetns.");
            this.fileCabinetService.Restore(snapshot);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.writer.Dispose();
            }

            this.disposed = true;
        }

        private void Print(string method, string parameters)
        {
            string message = $"Calling {method} with {parameters}.";

            this.writer.WriteLine($"{DateTime.Now} : {message}");
            Console.WriteLine(message);
        }
    }
}
