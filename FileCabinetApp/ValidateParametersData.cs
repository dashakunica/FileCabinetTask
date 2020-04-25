using System;
using System.Globalization;
using System.Text;

namespace FileCabinetApp
{
    public class ValidateParametersData
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public short Bonuses { get; set; }

        public decimal Salary { get; set; }

        public char AccountType { get; set; }

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
