using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Date of birth validator propertie.
    /// </summary>
    public class DateOfBirthValidator : IRecordValidator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DateOfBirthValidator"/> class.
        /// </summary>
        /// <param name="from">From valid value.</param>
        /// <param name="to">To valid value.</param>
        public DateOfBirthValidator(DateTime from, DateTime to)
        {
            this.From = from;
            this.To = to;
        }

        private DateTime From { get; set; }

        private DateTime To { get; set; }

        /// <inheritdoc/>
        public void ValidateParameters(ValidateParametersData data)
        {
            if (data is null)
            {
                throw new ArgumentNullException($"{nameof(data)} cannot be null.");
            }

            if (data.DateOfBirth < this.From || data.DateOfBirth > this.To)
            {
                throw new ArgumentException($"{nameof(data.DateOfBirth)} cannot be less then {this.From.Date} and more then {this.To.Date}.");
            }
        }
    }
}
