using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace FileCabinetApp
{
    public class ServiceMeter : IFileCabinetService
    {
        private readonly IFileCabinetService fileCabinetService;
        private readonly Stopwatch stopwatch;

        public ServiceMeter(IFileCabinetService fileCabinetService)
        {
            this.fileCabinetService = fileCabinetService ?? throw new ArgumentNullException(nameof(fileCabinetService));
            this.stopwatch = new Stopwatch();
        }

        public string Name => nameof(ServiceMeter);

        public int CreateRecord((string firstName, string lastName, DateTime dateOfBirth, short bonuses, decimal salary, char accountType) data)
        {
            this.stopwatch.Restart();
            var value = this.fileCabinetService.CreateRecord(data);
            this.stopwatch.Stop();
            Print(nameof(this.CreateRecord), this.stopwatch.ElapsedTicks);
            return value;
        }

        public int CreateRecordWithSpecifiedId(int id, (string firstName, string lastName, DateTime dateOfBirth, short bonuses, decimal salary, char accountType) data)
        {
            this.stopwatch.Restart();
            var value = this.fileCabinetService.CreateRecordWithSpecifiedId(id, data);
            this.stopwatch.Stop();
            Print(nameof(this.CreateRecord), this.stopwatch.ElapsedTicks);
            return value;
        }

        public void EditRecord(int id, (string firstName, string lastName, DateTime dateOfBirth, short bonuses, decimal salary, char accountType) data)
        {
            this.stopwatch.Restart();
            this.fileCabinetService.EditRecord(id, data);
            this.stopwatch.Stop();
            Print(nameof(this.EditRecord), this.stopwatch.ElapsedTicks);
        }

        public IEnumerable<FileCabinetRecord> FindByDateOfBirth(DateTime dateOfBirth)
        {
            this.stopwatch.Restart();
            var value = this.fileCabinetService.FindByDateOfBirth(dateOfBirth);
            this.stopwatch.Stop();
            Print(nameof(this.FindByDateOfBirth), this.stopwatch.ElapsedTicks);
            return value;
        }

        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
        {
            this.stopwatch.Restart();
            var value = this.fileCabinetService.FindByFirstName(firstName);
            this.stopwatch.Stop();
            Print(nameof(this.FindByFirstName), this.stopwatch.ElapsedTicks);
            return value;
        }

        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName)
        {
            this.stopwatch.Restart();
            var value = this.fileCabinetService.FindByLastName(lastName);
            this.stopwatch.Stop();
            Print(nameof(this.FindByLastName), this.stopwatch.ElapsedTicks);
            return value;
        }

        public IEnumerable<FileCabinetRecord> GetRecords()
        {
            this.stopwatch.Restart();
            var value = this.fileCabinetService.GetRecords();
            this.stopwatch.Stop();
            Print(nameof(this.GetRecords), this.stopwatch.ElapsedTicks);
            return value;
        }

        public (int active, int removed) GetStat()
        {
            this.stopwatch.Restart();
            var value = this.fileCabinetService.GetStat();
            this.stopwatch.Stop();
            Print(nameof(this.GetStat), this.stopwatch.ElapsedTicks);
            return value;
        }

        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            this.stopwatch.Restart();
            var value = this.fileCabinetService.MakeSnapshot();
            this.stopwatch.Stop();
            Print(nameof(this.MakeSnapshot), this.stopwatch.ElapsedTicks);
            return value;
        }

        public void Purge()
        {
            this.stopwatch.Restart();
            this.fileCabinetService.Purge();
            this.stopwatch.Stop();
            Print(nameof(this.Purge), this.stopwatch.ElapsedTicks);
        }

        public void RemoveRecord(int id)
        {
            this.stopwatch.Restart();
            this.fileCabinetService.RemoveRecord(id);
            this.stopwatch.Stop();
            Print(nameof(this.RemoveRecord), this.stopwatch.ElapsedTicks);
        }

        public void Restore(FileCabinetServiceSnapshot snapshot)
        {
            this.stopwatch.Restart();
            this.fileCabinetService.Restore(snapshot ?? throw new ArgumentNullException(nameof(snapshot)));
            this.stopwatch.Stop();
            Print(nameof(this.Restore), this.stopwatch.ElapsedTicks);
        }

        private static void Print(string name, long elapsedticks)
        {
            Console.WriteLine($"{name} method execution duration is {elapsedticks} ticks.");
        }
    }
}
