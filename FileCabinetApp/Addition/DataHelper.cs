using System;
using System.Globalization;

namespace FileCabinetApp
{
    public static class DataHelper
    {
        private const int MinNameLength = 2;
        private const int MaxNameLength = 60;

        private const short MinWorkPlaceNumber = 0;
        private const short MaxWorkPlaceNumber = 30_000;

        private const decimal MinSalary = 3_000;
        private const decimal MaxSalary = 100_000_000;

        private static DateTime MinDate => new DateTime(1900, 1, 1);

        private static DateTime MaxDate => DateTime.Now;

        public static FileCabinetRecord CreateRecordFromData(int id, (string firstName, string lastName, DateTime dateOfBirth, short bonuses, decimal salary, char accountType) data)
        {
            return new FileCabinetRecord()
            {
                Id = id,
                FirstName = data.firstName,
                LastName = data.lastName,
                DateOfBirth = data.dateOfBirth,
                Bonuses = data.bonuses,
                Salary = data.salary,
                AccountType = data.accountType,
            };
        }

        public static FileCabinetRecord GetData()
        {
            var data = new FileCabinetRecord();
            Console.Write("First Name: ");
            data.FirstName = ReadInput<string>(Convert<string>, x => x.Length < MinNameLength || x.Length > MaxNameLength
                                                                    ? new Tuple<bool, string>(false, nameof(data.FirstName))
                                                                    : new Tuple<bool, string>(true, nameof(data.FirstName)));
            Console.Write("Last Name: ");
            data.LastName = ReadInput<string>(Convert<string>, x => x.Length < MinNameLength || x.Length > MaxNameLength
                                                                    ? new Tuple<bool, string>(false, nameof(data.LastName))
                                                                    : new Tuple<bool, string>(true, nameof(data.LastName)));
            Console.Write("Date of birth: ");
            data.DateOfBirth = ReadInput<DateTime>(Convert<DateTime>, x => x < MinDate || x > MaxDate
                                                                    ? new Tuple<bool, string>(false, nameof(data.DateOfBirth))
                                                                    : new Tuple<bool, string>(true, nameof(data.DateOfBirth)));
            Console.Write("Bonuses: ");
            data.Bonuses = ReadInput<short>(Convert<short>, x => x < MinWorkPlaceNumber || x > MaxWorkPlaceNumber
                                                                    ? new Tuple<bool, string>(false, nameof(data.Bonuses))
                                                                    : new Tuple<bool, string>(true, nameof(data.Bonuses)));
            Console.Write("Salary: ");
            data.Salary = ReadInput<decimal>(Convert<decimal>, x => x < MinSalary || x > MaxSalary
                                                                    ? new Tuple<bool, string>(false, nameof(data.Salary))
                                                                    : new Tuple<bool, string>(true, nameof(data.Salary)));
            Console.Write("Account type: ");
            data.AccountType = ReadInput<char>(Convert<char>, x => char.IsLetterOrDigit(x)
                                                                    ? new Tuple<bool, string>(true, nameof(data.AccountType))
                                                                    : new Tuple<bool, string>(false, nameof(data.AccountType)));
            return data;
        }

        public static bool YesOrNo()
        {
            do
            {
                var yn = Console.ReadKey();

                if (yn.KeyChar == 'Y' || yn.KeyChar == 'y')
                {
                    Console.WriteLine();
                    return true;
                }

                if (yn.KeyChar == 'N' || yn.KeyChar == 'n')
                {
                    Console.WriteLine();
                    return false;
                }

                Console.Write($"{Environment.NewLine}Error: you can only chose [Y] yes or [N] no.");
            }
            while (true);
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

        private static Tuple<bool, string, T> Convert<T>(string arg)
        {
            bool converted = true;
            T result = default;
            try
            {
                if (arg is null)
                {
                    throw new ArgumentNullException(nameof(arg), $"{nameof(arg)} cannot be null.");
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
            catch (ArgumentNullException)
            {
                converted = false;
            }

            return new Tuple<bool, string, T>(converted, arg, result);
        }
    }
}
