using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    public class ValidatorBuilder
    {
        private readonly List<IRecordValidator> validators = new List<IRecordValidator>();

        public static IRecordValidator CreateDefault()
        {
            return new CompositeValidator(Enumerable.Repeat(new DefaultValidator(), 1));
        }

        public static IRecordValidator CreateCustom()
        {
            return new CompositeValidator(Enumerable.Repeat(new CustomValidator(), 1));
        }

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

        public ValidatorBuilder ValidateWorkPlaceNumber(short min, short max)
        {
            this.validators.Add(new WorkPlaceNumberValidator(min, max));
            return this;
        }

        public ValidatorBuilder ValidateSalary(decimal min, decimal max)
        {
            this.validators.Add(new SalaryValidator(min, max));
            return this;
        }

        public ValidatorBuilder ValidateDepartment()
        {
            this.validators.Add(new DepartmentValidator());
            return this;
        }

        public IRecordValidator Create()
        {
            return new CompositeValidator(this.validators);
        }
    }
}
