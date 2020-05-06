using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace FileCabinetApp
{
    /// <summary>
    /// Service logger.
    /// </summary>
    public class ServiceLogger : IFileCabinetService, IDisposable
    {
        private readonly IFileCabinetService fileCabinetService;
        private readonly StreamWriter writer;
        private bool disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceLogger"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Service.</param>
        /// <param name="path">Path.</param>
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

        /// <inheritdoc/>
        public int CreateAndSetId(ValidateParametersData data)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            this.Print(nameof(this.CreateAndSetId), data.ToString());
            var value = this.fileCabinetService.CreateAndSetId(data);
            this.Print(nameof(this.CreateAndSetId), value.ToString(CultureInfo.InvariantCulture));
            return value;
        }

        /// <inheritdoc/>
        public int CreateRecord(int id, ValidateParametersData data)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            this.Print(nameof(this.CreateAndSetId), data.ToString());
            var value = this.fileCabinetService.CreateRecord(id, data);
            this.Print(nameof(this.CreateAndSetId), value.ToString(CultureInfo.InvariantCulture));
            return value;
        }

        /// <inheritdoc/>
        public void EditRecord(int id, ValidateParametersData data)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            this.Print(nameof(this.EditRecord), $"{nameof(id)} = '{id.ToString(CultureInfo.InvariantCulture)}', {data.ToString()}");
            this.fileCabinetService.EditRecord(id, data);
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> GetRecords()
        {
            this.Print(nameof(this.GetRecords), string.Empty);
            var value = this.fileCabinetService.GetRecords();
            this.Print(nameof(this.GetRecords), $"{(value == null ? string.Empty : value.GetType().Name)}");
            return value;
        }

        /// <inheritdoc/>
        public (int active, int removed) GetStat()
        {
            this.Print(nameof(this.GetStat), string.Empty);
            var value = this.fileCabinetService.GetStat();
            this.Print(nameof(this.GetStat), $"({value.active}, {value.removed})");
            return value;
        }

        /// <inheritdoc/>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            this.Print(nameof(this.MakeSnapshot), string.Empty);
            var value = this.fileCabinetService.MakeSnapshot();
            this.Print(nameof(this.GetStat), $"snapshot with {value.Records.Count} elements.");
            return value;
        }

        /// <inheritdoc/>
        public void Purge()
        {
            this.Print(nameof(this.Purge), string.Empty);
            this.fileCabinetService.Purge();
        }

        /// <inheritdoc/>
        public void RemoveRecord(int id)
        {
            this.Print(nameof(this.RemoveRecord), $"{nameof(id)} = '{id}'");
            this.fileCabinetService.RemoveRecord(id);
        }

        /// <inheritdoc/>
        public void Restore(FileCabinetServiceSnapshot snapshot)
        {
            if (snapshot is null)
            {
                throw new ArgumentNullException(nameof(snapshot));
            }

            this.Print(nameof(this.Restore), $"snapshot wich contains {snapshot?.Records.Count} elemetns.");
            this.fileCabinetService.Restore(snapshot);
        }

        /// <inheritdoc/>
        public void Delete(IEnumerable<FileCabinetRecord> records)
        {
            this.Print(nameof(this.Restore), string.Empty);
            this.fileCabinetService.Delete(records);
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        /// <param name="disposing">True or false.</param>
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
