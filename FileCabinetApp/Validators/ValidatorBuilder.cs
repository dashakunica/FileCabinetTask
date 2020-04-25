using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace FileCabinetApp
{
    public class ValidatorBuilder
    {
        private const string Default = "default";
        private const string Custom = "custom";
        private const string FirstName = "firstName";
        private const string LastName = "lastName";
        private const string DateOfBirth = "dateOfBirth";
        private const string Bonuses = "bonuses";
        private const string Salary = "salary";

        private readonly List<IRecordValidator> validators = new List<IRecordValidator>();

        public static IRecordValidator CreateDefault() => CreateValidator(Default);

        public static IRecordValidator CreateCustom() => CreateValidator(Custom);

        public ValidatorBuilder ValidateFirstName(int min, int max)
        {
            this.validators.Add(new FirstNameValidator(min, max));
            return this;
        }

        public ValidatorBuilder ValidateLastName(int min, int max)
        {
            this.validators.Add(new LastNameValidator(min, max));
            return this;
        }

        public ValidatorBuilder ValidateDateOfBirth(DateTime from, DateTime to)
        {
            this.validators.Add(new DateOfBirthValidator(from, to));
            return this;
        }

        public ValidatorBuilder ValidateBonuses(short min, short max)
        {
            this.validators.Add(new BonusesValidator(min, max));
            return this;
        }

        public ValidatorBuilder ValidateSalary(decimal min, decimal max)
        {
            this.validators.Add(new SalaryValidator(min, max));
            return this;
        }

        public ValidatorBuilder ValidateAccountType()
        {
            this.validators.Add(new AccountTypeValidator());
            return this;
        }

        public IRecordValidator Create()
        {
            return new CompositeValidator(this.validators);
        }

        private static IRecordValidator CreateValidator(string type)
        {
            var firstName = Startup.Configuration.GetSection(type).GetSection(FirstName).Get<FirstNameJson>();
            var lastName = Startup.Configuration.GetSection(type).GetSection(LastName).Get<LastNameJson>();
            var dateOfBirth = Startup.Configuration.GetSection(type).GetSection(DateOfBirth).Get<DateOfBirthJson>();
            var workPlaceNumber = Startup.Configuration.GetSection(type).GetSection(Bonuses).Get<BonusesJson>();
            var salary = Startup.Configuration.GetSection(type).GetSection(Salary).Get<SalaryJson>();

            return new ValidatorBuilder()
                .ValidateFirstName(firstName.Min, firstName.Max)
                .ValidateLastName(lastName.Min, lastName.Max)
                .ValidateDateOfBirth(dateOfBirth.From, dateOfBirth.To)
                .ValidateBonuses(workPlaceNumber.Min, workPlaceNumber.Max)
                .ValidateSalary(salary.Min, salary.Max)
                .ValidateAccountType()
                .Create();
        }
    }
}
