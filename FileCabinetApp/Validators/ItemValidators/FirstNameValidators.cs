using System;

namespace FileCabinetApp
{
    public class FirstNameValidator : IRecordValidator
    {
        public FirstNameValidator(int min, int max)
        {
            this.Min = min;
            this.Max = max;
        }

        private int Min { get; set; }

        private int Max { get; set; }

        public void ValidateParameters(FileCabinetRecord data)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data), $"{nameof(data)} cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(data.FirstName))
            {
                throw new ArgumentNullException($"{nameof(data.FirstName)} cannot be null and contains only white spaces.");
            }

            if (data.FirstName.Length < this.Min || data.FirstName.Length > this.Max)
            {
                throw new ArgumentException(nameof(data.FirstName), $"{nameof(data.FirstName)} cannot be less then {this.Min} , more then {this.Max}.");
            }
        }
    }
}
