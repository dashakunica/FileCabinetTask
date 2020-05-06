using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace FileCabinetApp
{
    /// <summary>
    /// Builder of all validators.
    /// </summary>
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

        /// <summary>
        /// Gets json model first name.
        /// </summary>
        /// <value>
        /// Json model first name.
        /// </value>
        public static FirstNameJson FNameValidValue { get; private set; }

        /// <summary>
        /// Gets json model last name.
        /// </summary>
        /// <value>
        /// Json model last name.
        /// </value>
        public static LastNameJson LNameValidValue { get; private set; }

        /// <summary>
        /// Gets json model DoB.
        /// </summary>
        /// <value>
        /// Json model DoB.
        /// </value>
        public static DateOfBirthJson DoBValidValue { get; private set; }

        /// <summary>
        /// Gets json model bonuses.
        /// </summary>
        /// <value>
        /// Json model bonuses.
        /// </value>
        public static BonusesJson BonusesValidValue { get; private set; }

        /// <summary>
        /// Gets json model salary.
        /// </summary>
        /// <value>
        /// Json model salary.
        /// </value>
        public static SalaryJson SalaryValidValue { get; private set; }

        /// <summary>
        /// Create default validator.
        /// </summary>
        /// <returns>Validator.</returns>
        public static IRecordValidator CreateDefault() => CreateValidator(Default);

        /// <summary>
        /// Create custom validator.
        /// </summary>
        /// <returns>Validator.</returns>
        public static IRecordValidator CreateCustom() => CreateValidator(Custom);

        private static IRecordValidator CreateValidator(string type)
        {
            FNameValidValue = Startup.Configuration.GetSection(type).GetSection(FirstName).Get<FirstNameJson>();
            LNameValidValue = Startup.Configuration.GetSection(type).GetSection(LastName).Get<LastNameJson>();
            DoBValidValue = Startup.Configuration.GetSection(type).GetSection(DateOfBirth).Get<DateOfBirthJson>();
            BonusesValidValue = Startup.Configuration.GetSection(type).GetSection(Bonuses).Get<BonusesJson>();
            SalaryValidValue = Startup.Configuration.GetSection(type).GetSection(Salary).Get<SalaryJson>();

            return new ValidatorBuilder()
                .ValidateFirstName(FNameValidValue.Min, FNameValidValue.Max)
                .ValidateLastName(LNameValidValue.Min, LNameValidValue.Max)
                .ValidateDateOfBirth(DoBValidValue.Min, DoBValidValue.Max)
                .ValidateBonuses(BonusesValidValue.Min, BonusesValidValue.Max)
                .ValidateSalary(SalaryValidValue.Min, SalaryValidValue.Max)
                .ValidateAccountType()
                .Create();
        }

        private ValidatorBuilder ValidateFirstName(int min, int max)
        {
            this.validators.Add(new FirstNameValidator(min, max));
            return this;
        }

        private ValidatorBuilder ValidateLastName(int min, int max)
        {
            this.validators.Add(new LastNameValidator(min, max));
            return this;
        }

        private ValidatorBuilder ValidateDateOfBirth(DateTime from, DateTime to)
        {
            this.validators.Add(new DateOfBirthValidator(from, to));
            return this;
        }

        private ValidatorBuilder ValidateBonuses(short min, short max)
        {
            this.validators.Add(new BonusesValidator(min, max));
            return this;
        }

        private ValidatorBuilder ValidateSalary(decimal min, decimal max)
        {
            this.validators.Add(new SalaryValidator(min, max));
            return this;
        }

        private ValidatorBuilder ValidateAccountType()
        {
            this.validators.Add(new AccountTypeValidator());
            return this;
        }

        private IRecordValidator Create()
        {
            return new CompositeValidator(this.validators);
        }
    }
}
