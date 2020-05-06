using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Validator for salary propertie.
    /// </summary>
    public class SalaryValidator : IRecordValidator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SalaryValidator"/> class.
        /// </summary>
        /// <param name="min">Min valid value.</param>
        /// <param name="max">Max valid value.</param>
        public SalaryValidator(decimal min, decimal max)
        {
            this.Min = min;
            this.Max = max;
        }

        private decimal Min { get; set; }

        private decimal Max { get; set; }

        /// <inheritdoc/>
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
