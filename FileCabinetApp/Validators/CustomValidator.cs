using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    public class CustomValidator : IRecordValidator
    {
        public void ValidateParameter(FileCabinetRecord record)
        {
            if (record.FirstName is null)
            {
                throw new ArgumentNullException($"Parameter {nameof(record.FirstName)} cannot be null");
            }

            if (record.LastName is null)
            {
                throw new ArgumentNullException($"Parameter {nameof(record.LastName)} cannot be null");
            }

            if (record.FirstName.Length < 1 || record.FirstName.Length > 20)
            {
                throw new ArgumentException($"Parameter {nameof(record.FirstName)} cannot have length less than 3 or more than 60");
            }

            if (record.LastName.Length < 1 || record.LastName.Length > 20)
            {
                throw new ArgumentException($"Parameter {nameof(record.LastName)} cannot have length less than 3 or more than 60");
            }

            if (record.FirstName.Trim().Length == 0)
            {
                throw new ArgumentException($"Parameter {nameof(record.FirstName)} cannot have only whitespaces");
            }

            if (record.LastName.Trim().Length == 0)
            {
                throw new ArgumentException($"Parameter {nameof(record.LastName)} cannot have only whitespaces");
            }

            if (record.DateOfBirth.Day < new DateTime(1930, 01, 01).Day || record.DateOfBirth.Day > DateTime.Now.Day)
            {
                throw new ArgumentException($"Parameter {nameof(record.DateOfBirth)} cannot be less than 01-Jan-1950 and more than current day");
            }
        }

        public T ReadInput<T>(Func<string, Tuple<bool, string, T>> converter, Func<T, Tuple<bool, string>> validator)
        {
            do
            {
                T value;

                var input = Console.ReadLine();
                var conversionResult = converter(input);

                if (!conversionResult.Item1)
                {
                    Console.WriteLine($"Conversion failed: {conversionResult.Item2}. Please, correct your input.");
                    continue;
                }

                value = conversionResult.Item3;

                var validationResult = validator(value);
                if (!validationResult.Item1)
                {
                    Console.WriteLine($"Validation failed: {validationResult.Item2}. Please, correct your input.");
                    continue;
                }

                return value;
            }
            while (true);
        }
    }
}
