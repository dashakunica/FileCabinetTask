using System;

namespace FileCabinetApp
{
    public class DefaultValidator : IRecordValidator
    {
        private const int MinNameLength = 2;
        private const int MaxNameLength = 60;

        private const short MinBonuses = 0;
        private const short MaxBonuses = 30_000;

        private const decimal MinSalary = 3_000;
        private const decimal MaxSalary = 100_000_000;

        private static DateTime MinDate => new DateTime(1900, 1, 1);

        private static DateTime MaxDate => DateTime.Now;

        public static IRecordValidator Create()
        {
            return new DefaultValidator();
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
