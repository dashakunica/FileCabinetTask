using System;

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

        public void ValidateParameters(ValidateParametersData data)
        {
            if (data is null)
            {
                throw new ArgumentNullException($"{nameof(data)} cannot be null.");
            }

            if (data.Salary < this.Min || data.Salary > this.Max)
            {
                throw new ArgumentException($"{nameof(data.Salary)} cannot be less than {this.Min} and more than {this.Max}");
            }
        }
    }
}
