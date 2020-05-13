using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace FileCabinetApp
{
    /// <summary>
    /// Class helper for working with data.
    /// </summary>
    public static class DataHelper
    {
        private static readonly FirstNameJson FirstName = ValidatorBuilder.FNameValidValue;
        private static readonly LastNameJson LastName = ValidatorBuilder.LNameValidValue;
        private static readonly DateOfBirthJson DateOfBirth = ValidatorBuilder.DoBValidValue;
        private static readonly BonusesJson Bonuses = ValidatorBuilder.BonusesValidValue;
        private static readonly SalaryJson Salary = ValidatorBuilder.SalaryValidValue;

        private static readonly PropertyInfo[] FileCabinetProperties = typeof(FileCabinetRecord).GetProperties();

        /// <summary>
        /// Request data from user in console.
        /// </summary>
        /// <returns>Validate parameters data from user input.</returns>
        public static ValidateParametersData RequestData()
        {
            var data = new ValidateParametersData();
            Console.Write("First Name: ");
            data.FirstName = ReadInput<string>(Convert<string>, x => x.Length < FirstName.Min || x.Length > FirstName.Max
                                                                    ? new Tuple<bool, string>(false, nameof(data.FirstName))
                                                                    : new Tuple<bool, string>(true, nameof(data.FirstName)));
            Console.Write("Last Name: ");
            data.LastName = ReadInput<string>(Convert<string>, x => x.Length < LastName.Min || x.Length > LastName.Max
                                                                    ? new Tuple<bool, string>(false, nameof(data.LastName))
                                                                    : new Tuple<bool, string>(true, nameof(data.LastName)));
            Console.Write("Date of birth: ");
            data.DateOfBirth = ReadInput<DateTime>(Convert<DateTime>, x => x < DateOfBirth.From || x > DateOfBirth.To
                                                                    ? new Tuple<bool, string>(false, nameof(data.DateOfBirth))
                                                                    : new Tuple<bool, string>(true, nameof(data.DateOfBirth)));
            Console.Write("Work place number: ");
            data.Bonuses = ReadInput<short>(Convert<short>, x => x < Bonuses.Min || x > Bonuses.Max
                                                                    ? new Tuple<bool, string>(false, nameof(data.Bonuses))
                                                                    : new Tuple<bool, string>(true, nameof(data.Bonuses)));
            Console.Write($"Salary(min: {Salary.Min} max: {Salary.Max}): ");
            data.Salary = ReadInput<decimal>(Convert<decimal>, x => x < Salary.Min || x > Salary.Max
                                                                    ? new Tuple<bool, string>(false, nameof(data.Salary))
                                                                    : new Tuple<bool, string>(true, nameof(data.Salary)));
            Console.Write("Department: ");
            data.AccountType = ReadInput<char>(Convert<char>, x => char.IsLetterOrDigit(x)
                                                                    ? new Tuple<bool, string>(true, nameof(data.AccountType))
                                                                    : new Tuple<bool, string>(false, nameof(data.AccountType)));
            return data;
        }

        /// <summary>
        /// Convert FileCabinetRecord data into ValidateParametersData.
        /// </summary>
        /// <param name="record">Data to convert.</param>
        /// <returns>Validate parameters data.</returns>
        public static ValidateParametersData CreateValidateData(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            return new ValidateParametersData()
            {
                FirstName = record.FirstName is null ? null : record.FirstName,
                LastName = record.LastName is null ? null : record.LastName,
                DateOfBirth = record.DateOfBirth,
                Bonuses = record.Bonuses,
                Salary = record.Salary,
                AccountType = record.AccountType,
            };
        }

        /// <summary>
        /// Convert validate parameters data into file cabinet record model.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <param name="data">Data.</param>
        /// <returns>File cabinet record model.</returns>
        public static FileCabinetRecord CreateRecordFromArgs(int id, ValidateParametersData data)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            return new FileCabinetRecord()
            {
                Id = id,
                FirstName = data.FirstName,
                LastName = data.LastName,
                DateOfBirth = data.DateOfBirth,
                Bonuses = data.Bonuses,
                Salary = data.Salary,
                AccountType = data.AccountType,
            };
        }

        /// <summary>
        /// Update record from data.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <param name="data">Data.</param>
        /// <param name="record">Record.</param>
        public static void UpdateRecordFromData(int id, ValidateParametersData data, FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            record.Id = id;
            record.FirstName = data.FirstName;
            record.LastName = data.LastName;
            record.DateOfBirth = data.DateOfBirth;
            record.Bonuses = data.Bonuses;
            record.Salary = data.Salary;
            record.AccountType = data.AccountType;
        }

        /// <summary>
        /// Create record from value propertie pair.
        /// </summary>
        /// <param name="propNewValues">Dictionary with propertie value pair.</param>
        /// <returns>Record model.</returns>
        public static FileCabinetRecord CreateRecordFromDict(Dictionary<string, string> propNewValues)
        {
            if (propNewValues is null)
            {
                throw new ArgumentNullException(nameof(propNewValues));
            }

            var arg = new FileCabinetRecord();
            foreach (var item in propNewValues)
            {
                var prop = FileCabinetProperties.FirstOrDefault(x => x.Name.Equals(item.Key, StringComparison.InvariantCultureIgnoreCase));
                var converter = TypeDescriptor.GetConverter(prop?.PropertyType);
                prop.SetValue(arg, converter.ConvertFromString(item.Value));
            }

            return arg;
        }

        /// <summary>
        /// Yes or no input message.
        /// </summary>
        /// <returns>True or false.</returns>
        public static bool YesOrNo()
        {
            do
            {
                var input = Console.ReadKey();

                if (input.KeyChar == 'Y' || input.KeyChar == 'y')
                {
                    Console.WriteLine();
                    return true;
                }

                if (input.KeyChar == 'N' || input.KeyChar == 'n')
                {
                    Console.WriteLine();
                    return false;
                }

                Console.WriteLine($"You can only choose Y (yes) or N (no).");
            }
            while (true);
        }

        /// <summary>
        /// Calculate similarity of 2 string.
        /// </summary>
        /// <param name="source">string 1.</param>
        /// <param name="target">string 2.</param>
        /// <returns>Percent of similarity.</returns>
        public static double GetSimilarity(string source, string target)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(target))
            {
                return 0;
            }

            var sourceLength = source.Length;
            var targetLength = target.Length;
            int stepsToSame;

            int[,] matrix = new int[sourceLength + 1, targetLength + 1];

            if (sourceLength == 0)
            {
                stepsToSame = targetLength;
            }
            else if (targetLength == 0)
            {
                stepsToSame = sourceLength;
            }
            else
            {
                for (var i = 0; i <= sourceLength; i++)
                {
                    matrix[i, 0] = i;
                }

                for (var j = 0; j <= targetLength; j++)
                {
                    matrix[0, j] = j;
                }

                for (var i = 1; i <= sourceLength; i++)
                {
                    for (var j = 1; j <= targetLength; j++)
                    {
                        var cost = (target[j - 1] == source[i - 1]) ? 0 : 1;

                        matrix[i, j] = Math.Min(
                            Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
                            matrix[i - 1, j - 1] + cost);
                    }
                }

                stepsToSame = matrix[sourceLength, targetLength];
            }

            return 1 - ((double)stepsToSame / (double)Math.Max(source.Length, target.Length));
        }

        private static T ReadInput<T>(Func<string, Tuple<bool, string, T>> converter, Func<T, Tuple<bool, string>> validator)
        {
            do
            {
                T value;

                var input = Console.ReadLine();
                var conversionResult = converter(input);

                if (!conversionResult.Item1)
                {
                    Console.WriteLine($"Cannot convert parameter {conversionResult.Item2}. Please, correct your input.");
                    continue;
                }

                value = conversionResult.Item3;

                var validationResult = validator(value);
                if (!validationResult.Item1)
                {
                    Console.WriteLine($"It is not valid parameter: {validationResult.Item2}. Please, correct your input.");
                    continue;
                }

                return value;
            }
            while (true);
        }

        private static Tuple<bool, string, T> Convert<T>(string arg)
        {
            bool converted = true;
            T result = default;
            try
            {
                if (arg is null)
                {
                    converted = false;
                }

                result = (T)System.Convert.ChangeType(arg, typeof(T), CultureInfo.InvariantCulture);
            }
            catch (InvalidCastException)
            {
                converted = false;
            }
            catch (FormatException)
            {
                converted = false;
            }
            catch (OverflowException)
            {
                converted = false;
            }

            return new Tuple<bool, string, T>(converted, arg, result);
        }
    }
}
