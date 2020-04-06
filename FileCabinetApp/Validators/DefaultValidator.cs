using System;
using Microsoft.Extensions.Configuration;

namespace FileCabinetApp
{
    public class DefaultValidator : IRecordValidator
    {
        private const string Default = "default";
        private const string FirstName = "firstName";
        private const string LastName = "lastName";
        private const string DateOfBirth = "dateOfBirth";
        private const string WorkPlaceNumber = "workPlaceNumber";
        private const string Salary = "salary";

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

            var firstName = Startup.Configuration.GetSection(Default).GetSection(FirstName).Get<FirstNameJson>();
            var lastName = Startup.Configuration.GetSection(Default).GetSection(LastName).Get<LastNameJson>();
            var dateOfBirth = Startup.Configuration.GetSection(Default).GetSection(DateOfBirth).Get<DateOfBirthJson>();
            var workPlaceNumber = Startup.Configuration.GetSection(Default).GetSection(WorkPlaceNumber).Get<BonusesJson>();
            var salary = Startup.Configuration.GetSection(Default).GetSection(Salary).Get<SalaryJson>();

            new FirstNameValidator(firstName.Min, firstName.Max).ValidateParameters(data);
            new LastNameValidator(lastName.Min, lastName.Max).ValidateParameters(data);
            new DateOfBirthValidator(dateOfBirth.From, dateOfBirth.To).ValidateParameters(data);
            new BonusesValidator(workPlaceNumber.Min, workPlaceNumber.Max).ValidateParameters(data);
            new SalaryValidator(salary.Min, salary.Max).ValidateParameters(data);
            new AccountTypeValidator().ValidateParameters(data);
        }
    }
}
