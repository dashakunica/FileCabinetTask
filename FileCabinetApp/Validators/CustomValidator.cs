using System;

namespace FileCabinetApp
{
    public class CustomValidator : IRecordValidator
    {
        private const int MinNameLength = 2;
        private const int MaxNameLength = 50;

        private const short MinBonuses = 0;
        private const short MaxBonuses = 25_000;

        private const decimal MinSalary = 1_000;
        private const decimal MaxSalary = 1000_000_000;

        private static DateTime MinDate => new DateTime(1900, 1, 1);

        private static DateTime MaxDate => DateTime.Now;

        public static IRecordValidator Create()
        {
            return new CustomValidator();
        }

        public void ValidateParameters(FileCabinetRecord data)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data), $"{nameof(data)} cannot be null.");
            }

            new FirstNameValidator(MinNameLength, MaxNameLength).ValidateParameters(data);
            new LastNameValidator(MinNameLength, MaxNameLength).ValidateParameters(data);
            new DateOfBirthValidator(MinDate, MaxDate).ValidateParameters(data);
            new BonusesValidator(MinBonuses, MaxBonuses).ValidateParameters(data);
            new SalaryValidator(MinSalary, MaxSalary).ValidateParameters(data);
            new AccountTypeValidator().ValidateParameters(data);
        }
    }
}
