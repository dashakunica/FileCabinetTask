using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace FileCabinetApp
{
    /// <summary>
    /// Service meter.
    /// </summary>
    public class ServiceMeter : IFileCabinetService
    {
        private readonly IFileCabinetService fileCabinetService;
        private readonly Stopwatch stopwatch;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceMeter"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Service.</param>
        public ServiceMeter(IFileCabinetService fileCabinetService)
        {
            this.fileCabinetService = fileCabinetService ?? throw new ArgumentNullException(nameof(fileCabinetService));
            this.stopwatch = new Stopwatch();
        }

        /// <inheritdoc/>
        public int CreateAndSetId(ValidateParametersData data)
        {
            this.stopwatch.Restart();
            var value = this.fileCabinetService.CreateAndSetId(data);
            this.stopwatch.Stop();
            Print(nameof(this.CreateAndSetId), this.stopwatch.ElapsedTicks);
            return value;
        }

        /// <inheritdoc/>
        public int CreateRecord(int id, ValidateParametersData data)
        {
            this.stopwatch.Restart();
            var value = this.fileCabinetService.CreateRecord(id, data);
            this.stopwatch.Stop();
            Print(nameof(this.CreateAndSetId), this.stopwatch.ElapsedTicks);
            return value;
        }

        /// <inheritdoc/>
        public void EditRecord(int id, ValidateParametersData data)
        {
            this.stopwatch.Restart();
            this.fileCabinetService.EditRecord(id, data);
            this.stopwatch.Stop();
            Print(nameof(this.EditRecord), this.stopwatch.ElapsedTicks);
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> GetRecords()
        {
            this.stopwatch.Restart();
            var value = this.fileCabinetService.GetRecords();
            this.stopwatch.Stop();
            Print(nameof(this.GetRecords), this.stopwatch.ElapsedTicks);
            return value;
        }

        /// <inheritdoc/>
        public (int active, int removed) GetStat()
        {
            this.stopwatch.Restart();
            var value = this.fileCabinetService.GetStat();
            this.stopwatch.Stop();
            Print(nameof(this.GetStat), this.stopwatch.ElapsedTicks);
            return value;
        }

        /// <inheritdoc/>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            this.stopwatch.Restart();
            var value = this.fileCabinetService.MakeSnapshot();
            this.stopwatch.Stop();
            Print(nameof(this.MakeSnapshot), this.stopwatch.ElapsedTicks);
            return value;
        }

        /// <inheritdoc/>
        public void Purge()
        {
            this.stopwatch.Restart();
            this.fileCabinetService.Purge();
            this.stopwatch.Stop();
            Print(nameof(this.Purge), this.stopwatch.ElapsedTicks);
        }

        /// <inheritdoc/>
        public void RemoveRecord(int id)
        {
            this.stopwatch.Restart();
            this.fileCabinetService.RemoveRecord(id);
            this.stopwatch.Stop();
            Print(nameof(this.RemoveRecord), this.stopwatch.ElapsedTicks);
        }

        /// <inheritdoc/>
        public void Restore(FileCabinetServiceSnapshot snapshot)
        {
            this.stopwatch.Restart();
            this.fileCabinetService.Restore(snapshot ?? throw new ArgumentNullException(nameof(snapshot)));
            this.stopwatch.Stop();
            Print(nameof(this.Restore), this.stopwatch.ElapsedTicks);
        }

        /// <inheritdoc/>
        public void Delete(IEnumerable<FileCabinetRecord> records)
        {
            this.stopwatch.Restart();
            this.fileCabinetService.Delete(records);
            this.stopwatch.Stop();
            Print(nameof(this.Delete), this.stopwatch.ElapsedTicks);
        }

        private static void Print(string name, long elapsedticks)
        {
            Console.WriteLine($"{name} method execution duration is {elapsedticks} ticks.");
        }
    }
}
