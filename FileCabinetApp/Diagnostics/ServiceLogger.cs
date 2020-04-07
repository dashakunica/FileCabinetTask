using System;
using System.Collections.ObjectModel;
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

        private enum LogMessageType
        {
            CallMethodWithArguments,
            CallMethodWithoutArguments,
            ReturnValue,
        }

        public string Name => nameof(ServiceLogger);

        public int CreateRecord((string firstName, string lastName, DateTime dateOfBirth, short bonuses, decimal salary, char accountType) data)
        {
            this.Print(LogMessageType.CallMethodWithArguments, nameof(this.CreateRecord), data.ToString());
            var value = this.fileCabinetService.CreateRecord(data);
            this.Print(LogMessageType.ReturnValue, nameof(this.CreateRecord), value.ToString(CultureInfo.InvariantCulture));
            return value;
        }

        public void EditRecord(int id, (string firstName, string lastName, DateTime dateOfBirth, short bonuses, decimal salary, char accountType) data)
        {
            this.Print(LogMessageType.CallMethodWithArguments, nameof(this.EditRecord), $"{nameof(id)} = '{id.ToString(CultureInfo.InvariantCulture)}', {data.ToString()}");
            this.fileCabinetService.EditRecord(id, data);
        }

        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(DateTime dateOfBirth)
        {
            this.Print(LogMessageType.CallMethodWithArguments, nameof(this.FindByDateOfBirth), $"{nameof(dateOfBirth)} = '{dateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture)}'");
            var value = this.fileCabinetService.FindByDateOfBirth(dateOfBirth);
            this.Print(LogMessageType.ReturnValue, nameof(this.FindByDateOfBirth), $"{value.GetType().Name} with {value.Count} elements.");
            return value;
        }

        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            this.Print(LogMessageType.CallMethodWithArguments, nameof(this.FindByDateOfBirth), $"{nameof(firstName)} = '{firstName}'");
            var value = this.fileCabinetService.FindByFirstName(firstName);
            this.Print(LogMessageType.ReturnValue, nameof(this.FindByFirstName), $"{value.GetType().Name} with {value.Count} elements.");
            return value;
        }

        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            this.Print(LogMessageType.CallMethodWithArguments, nameof(this.FindByLastName), $"{nameof(lastName)} = '{lastName}'");
            var value = this.fileCabinetService.FindByLastName(lastName);
            this.Print(LogMessageType.ReturnValue, nameof(this.FindByLastName), $"{value.GetType().Name} with {value.Count} elements.");
            return value;
        }

        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            this.Print(LogMessageType.CallMethodWithoutArguments, nameof(this.GetRecords), string.Empty);
            var value = this.fileCabinetService.GetRecords();
            this.Print(LogMessageType.ReturnValue, nameof(this.GetRecords), $"{value.GetType().Name} with {value.Count} elements.");
            return value;
        }

        public (int active, int removed) GetStat()
        {
            this.Print(LogMessageType.CallMethodWithoutArguments, nameof(this.GetStat), string.Empty);
            var value = this.fileCabinetService.GetStat();
            this.Print(LogMessageType.ReturnValue, nameof(this.GetStat), $"({value.active}, {value.removed})");
            return value;
        }

        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            this.Print(LogMessageType.CallMethodWithoutArguments, nameof(this.MakeSnapshot), string.Empty);
            var value = this.fileCabinetService.MakeSnapshot();
            this.Print(LogMessageType.ReturnValue, nameof(this.GetStat), $"snapshot with {value.Records.Count} elements.");
            return value;
        }

        public void Purge()
        {
            this.Print(LogMessageType.CallMethodWithoutArguments, nameof(this.Purge), string.Empty);
            this.fileCabinetService.Purge();
        }

        public void RemoveRecord(int id)
        {
            this.Print(LogMessageType.CallMethodWithArguments, nameof(this.RemoveRecord), $"{nameof(id)} = '{id}'");
            this.fileCabinetService.RemoveRecord(id);
        }

        public void Restore(FileCabinetServiceSnapshot snapshot)
        {
            if (snapshot is null)
            {
                throw new ArgumentNullException(nameof(snapshot));
            }

            this.Print(LogMessageType.CallMethodWithArguments, nameof(this.Restore), $"snapshot wich contains {snapshot?.Records.Count} elemetns.");
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

        private void Print(LogMessageType messageType, string method, string parameters)
        {
            string message = string.Empty;
            switch (messageType)
            {
                case LogMessageType.CallMethodWithArguments:
                    {
                        message = $"Calling {method} with {parameters}.";
                        break;
                    }

                case LogMessageType.CallMethodWithoutArguments:
                    {
                        message = $"Calling {method}.";
                        break;
                    }

                case LogMessageType.ReturnValue:
                    {
                        message = $"{method} return {parameters}";
                        break;
                    }
            }

            this.writer.WriteLine($"{DateTime.Now} : {message}");
            Console.WriteLine(message);
        }
    }
}
