using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    public class CreateCommandHandler : ServiceCommandHandlerBase
    {
        private static void Create(string parameters)
        {
            if (string.IsNullOrEmpty(parameters))
            {
                var data = GetData();

                fileCabinetService.CreateRecord(data);

                var recordsCount = Program.fileCabinetService.GetStat();
                Console.WriteLine($"Record #{recordsCount} is created.");
            }

            Console.WriteLine();
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

        private static (string, string, DateTime, short, decimal, char) GetData()
        {
            Console.Write("First Name: ");
            var firstName = ReadInput<string>(Converter.StringConverter, validator.ValidateFirstName);
            Console.Write("Last Name: ");
            var lastName = ReadInput<string>(Converter.StringConverter, validator.ValidateLastName);
            Console.Write("Date of birth: ");
            var dateOfBirth = ReadInput<DateTime>(Converter.DateOfBirthConverter, validator.ValidateDateOfBirth);
            Console.Write("Bonuses: ");
            var bonuses = ReadInput<short>(Converter.BonusesConverter, validator.ValidateBonuses);
            Console.Write("Salary: ");
            var salary = ReadInput<decimal>(Converter.SalaryConverter, validator.ValidateSalary);
            Console.Write("Account type: ");
            var accountType = ReadInput<char>(Converter.AccountTypeConverter, validator.ValidateAccountType);

            return (firstName, lastName, dateOfBirth, bonuses, salary, accountType);
        }
    }
}
