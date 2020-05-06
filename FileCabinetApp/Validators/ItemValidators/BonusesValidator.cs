using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Validator for bonuses propertie.
    /// </summary>
    public class BonusesValidator : IRecordValidator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BonusesValidator"/> class.
        /// </summary>
        /// <param name="min">Min valid value.</param>
        /// <param name="max">Max valid value.</param>
        public BonusesValidator(short min, short max)
        {
            this.Min = min;
            this.Max = max;
        }

        private short Min { get; set; }

        private short Max { get; set; }

        /// <inheritdoc/>
        public void ValidateParameters(ValidateParametersData data)
        {
            if (data is null)
            {
                throw new ArgumentNullException($"{nameof(data)} cannot be null.");
            }

            if (data.Bonuses < this.Min || data.Bonuses > this.Max)
            {
                throw new ArgumentException($"{nameof(data.Bonuses)} cannot be less then {this.Min} and more then {this.Max}");
            }
        }
    }
}
