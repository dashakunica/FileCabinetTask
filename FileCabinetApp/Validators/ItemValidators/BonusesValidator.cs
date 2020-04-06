using System;

namespace FileCabinetApp
{
    public class BonusesValidator : IRecordValidator
    {
        public BonusesValidator(short min, short max)
        {
            this.Min = min;
            this.Max = max;
        }

        private short Min { get; set; }

        private short Max { get; set; }

        public void ValidateParameters(FileCabinetRecord data)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data), $"{nameof(data)} cannot be null.");
            }

            if (data.Bonuses < this.Min || data.Bonuses > this.Max)
            {
                throw new ArgumentException(nameof(data.Bonuses), $"{nameof(data.Bonuses)} cannot be less then {this.Min} and more then {this.Max}");
            }
        }
    }
}
