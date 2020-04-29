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

        public static FileCabinetRecord CreateRecordFromArgs(int id, ValidateParametersData data)
        {
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

        public static ValidateParametersData GetData()
        {
            var data = new ValidateParametersData();
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
                    Console.WriteLine($"It is not valid parameter {conversionResult.Item2}. Please, correct your input.");
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
                    throw new ArgumentNullException(nameof(arg), $"{nameof(arg)} cannot be null.");
                }

                result = (T)System.Convert.ChangeType(arg, typeof(T), CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                converted = false;
            }

            return new Tuple<bool, string, T>(converted, arg, result);
        }

        public static double CalculateSimilarity(string source, string target)
        {
            if ((source == null) || (target == null))
            {
                return 0;
            }

            if ((source.Length == 0) || (target.Length == 0))
            {
                return 0;
            }

            if (source == target)
            {
                return 1;
            }

            int stepsToSame = LevenshteinDistance(source, target);
            return 1 - ((double)stepsToSame / (double)Math.Max(source.Length, target.Length));
        }

        private static int LevenshteinDistance(string source, string target)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            int diff;
            var m = new int[source.Length][];

            for (int i = 0; i < source.Length; i++)
            {
                m[i] = new int[target.Length];
                m[i][0] = i;
            }

            for (int j = 0; j < target.Length; j++)
            {
                m[0][j] = j;
            }

            for (int i = 1; i < source.Length; i++)
            {
                for (int j = 1; j < target.Length; j++)
                {
                    diff = (source[i - 1] == target[j - 1]) ? 0 : 1;
                    m[i][j] = Math.Min(Math.Min(m[i - 1][j] + 1, m[i][j - 1] + 1), m[i - 1][j - 1] + diff);
                }
            }

            return m[source.Length - 1][target.Length - 1];
        }

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
    }
}
