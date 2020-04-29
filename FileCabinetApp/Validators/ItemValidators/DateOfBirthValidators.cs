using System;

namespace FileCabinetApp
{
    public class DateOfBirthValidator : IRecordValidator
    {
        public DateOfBirthValidator(DateTime from, DateTime to)
        {
            this.From = from;
            this.To = to;
        }

        private DateTime From { get; set; }

        private DateTime To { get; set; }

        public void ValidateParameters(ValidateParametersData data)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data), $"{nameof(data)} cannot be null.");
            }

            if (data.DateOfBirth < this.From || data.DateOfBirth > this.To)
            {
                throw new ArgumentException(nameof(data.DateOfBirth), $"{nameof(data.DateOfBirth)} cannot be less then {this.From.Date} and more then {this.To.Date}.");
            }
        }
    }
}
