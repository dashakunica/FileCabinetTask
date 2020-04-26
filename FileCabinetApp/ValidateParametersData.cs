using System;
using System.Globalization;
using System.Text;

namespace FileCabinetApp
{
    public class ValidateParametersData : ICloneable
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public short Bonuses { get; set; }

        public decimal Salary { get; set; }

        public char AccountType { get; set; }

        public ValidateParametersData Clone() => new ValidateParametersData()
        {
            FirstName = this.FirstName,
            LastName = this.LastName,
            DateOfBirth = this.DateOfBirth,
            Bonuses = this.Bonuses,
            Salary = this.Salary,
            AccountType = this.AccountType,
        };

        object ICloneable.Clone() => this.Clone();

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append($"{this.FirstName}, ");
            builder.Append($"{this.LastName}, ");
            builder.Append($"{this.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture)}, ");
            builder.Append($"{this.Bonuses}, ");
            builder.Append($"{this.Salary}, ");
            builder.Append($"{this.AccountType}");
            return builder.ToString();
        }
    }
}
