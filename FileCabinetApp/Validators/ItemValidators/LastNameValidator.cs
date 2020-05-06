using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Validator for last name propertie.
    /// </summary>
    public class LastNameValidator : IRecordValidator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LastNameValidator"/> class.
        /// </summary>
        /// <param name="min">Min valid value.</param>
        /// <param name="max">Max valid value.</param>
        public LastNameValidator(int min, int max)
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

            if (string.IsNullOrWhiteSpace(data.LastName))
            {
                throw new ArgumentNullException($"{nameof(data.LastName)} cannot be null and contains only white spaces.");
            }

            if (data.LastName.Length < this.Min || data.LastName.Length > this.Max)
            {
                throw new ArgumentException($"{nameof(data.LastName)} cannot be less than {this.Min}, more than {this.Max}.");
            }
        }
    }
}
