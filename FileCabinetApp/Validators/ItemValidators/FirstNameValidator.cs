using System;

namespace FileCabinetApp
{
    /// <summary>
    /// First name validator.
    /// </summary>
    public class FirstNameValidator : IRecordValidator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FirstNameValidator"/> class.
        /// </summary>
        /// <param name="min">Min valid value.</param>
        /// <param name="max">Max valid value.</param>
        public FirstNameValidator(int min, int max)
        {
            this.Min = min;
            this.Max = max;
        }

        private int Min { get; set; }

        private int Max { get; set; }

        /// <inheritdoc/>
        public void ValidateParameters(ValidateParametersData data)
        {
            if (data is null)
            {
                throw new ArgumentNullException($"{nameof(data)} cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(data.FirstName))
            {
                throw new ArgumentNullException($"{nameof(data.FirstName)} cannot be null and contains only white spaces.");
            }

            if (data.FirstName.Length < this.Min || data.FirstName.Length > this.Max)
            {
                throw new ArgumentException($"{nameof(data.FirstName)} cannot be less then {this.Min} , more then {this.Max}.");
            }
        }
    }
}
