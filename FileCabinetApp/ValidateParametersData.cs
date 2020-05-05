using System;
using System.Globalization;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Model for validation data of record.
    /// </summary>
    public class ValidateParametersData : ICloneable
    {
        /// <summary>
        /// Gets or sets firstname for validateData.
        /// </summary>
        /// <value>
        /// Firstname.
        /// </value>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets lastname for validateData.
        /// </summary>
        /// <value>
        /// Lastname.
        /// </value>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets date of birth for validateData.
        /// </summary>
        /// <value>
        /// Date of birth.
        /// </value>
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets bonuses for validateData.
        /// </summary>
        /// <value>
        /// Bonuses.
        /// </value>
        public short Bonuses { get; set; }

        /// <summary>
        /// Gets or sets salary for validateData.
        /// </summary>
        /// <value>
        /// Salary.
        /// </value>
        public decimal Salary { get; set; }

        /// <summary>
        /// Gets or sets account type for validateData.
        /// </summary>
        /// <value>
        /// Account type.
        /// </value>
        public char AccountType { get; set; }

        /// <summary>
        /// For clone data.
        /// </summary>
        /// <returns>Cloning data.</returns>
        public ValidateParametersData Clone() => new ValidateParametersData()
        {
            FirstName = this.FirstName,
            LastName = this.LastName,
            DateOfBirth = this.DateOfBirth,
            Bonuses = this.Bonuses,
            Salary = this.Salary,
            AccountType = this.AccountType,
        };

        /// <summary>
        /// For IClonable interface implementation.
        /// </summary>
        /// <returns>Clone object.</returns>
        object ICloneable.Clone() => this.Clone();

        /// <summary>
        /// Override method for represents record as string.
        /// </summary>
        /// <returns>String.</returns>
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
