using System;
using Microsoft.Extensions.Configuration;

namespace FileCabinetApp
{
    public class CustomValidator : IRecordValidator
    {
        private const string Custom = "custom";
        private const string FirstName = "firstName";
        private const string LastName = "lastName";
        private const string DateOfBirth = "dateOfBirth";
        private const string Bonuses = "bonuses";
        private const string Salary = "salary";

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

            var firstName = Startup.Configuration.GetSection(Custom).GetSection(FirstName).Get<FirstNameJson>();
            var lastName = Startup.Configuration.GetSection(Custom).GetSection(LastName).Get<LastNameJson>();
            var dateOfBirth = Startup.Configuration.GetSection(Custom).GetSection(DateOfBirth).Get<DateOfBirthJson>();
            var bonuses = Startup.Configuration.GetSection(Custom).GetSection(Bonuses).Get<BonusesJson>();
            var salary = Startup.Configuration.GetSection(Custom).GetSection(Salary).Get<SalaryJson>();

            new FirstNameValidator(firstName.Min, firstName.Max).ValidateParameters(data);
            new LastNameValidator(lastName.Min, lastName.Max).ValidateParameters(data);
            new DateOfBirthValidator(dateOfBirth.From, dateOfBirth.To).ValidateParameters(data);
            new BonusesValidator(bonuses.Min, bonuses.Max).ValidateParameters(data);
            new SalaryValidator(salary.Min, salary.Max).ValidateParameters(data);
            new AccountTypeValidator().ValidateParameters(data);
        }
    }
}
