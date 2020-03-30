using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    public class SalaryValidator : IRecordValidator
    {
        public SalaryValidator(decimal min, decimal max)
        {
            this.Min = min;
            this.Max = max;
        }

        private decimal Min { get; set; }

        private decimal Max { get; set; }

        public void ValidateParameters(FileCabinetRecord data)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data), $"{nameof(data)} cannot be null.");
            }

            if (data.Salary < this.Min || data.Salary > this.Max)
            {
                throw new ArgumentException(nameof(data.Salary), $"{nameof(data.Salary)} cannot be less then {this.Min} and more then {this.Max}");
            }
        }
    }
}
