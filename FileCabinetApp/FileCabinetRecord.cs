using System;
using System.Globalization;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Class for cabinet record type.
    /// </summary>
    public class FileCabinetRecord
    {
        /// <summary>
        /// Gets or sets Id of record.
        /// </summary>
        /// <value>
        /// Id.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets FirstName of record.
        /// </summary>
        /// <value>
        /// FirstName.
        /// </value>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets LastName of record.
        /// </summary>
        /// <value>
        /// LastName.
        /// </value>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets DateOfBirth of record.
        /// </summary>
        /// <value>
        /// DateOfBirth.
        /// </value>
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets Bonuses of record.
        /// </summary>
        /// <value>
        /// Bonuses.
        /// </value>
        public short Bonuses { get; set; }

        /// <summary>
        /// Gets or sets Salary of record.
        /// </summary>
        /// <value>
        /// Salary.
        /// </value>
        public decimal Salary { get; set; }

        /// <summary>
        /// Gets or sets AccountType of record.
        /// </summary>
        /// <value>
        /// AccountType.
        /// </value>
        public char AccountType { get; set; }

        /// <summary>
        /// Override method for string representation of record.
        /// </summary>
        /// <returns>String representation of record.</returns>
        public override string ToString() => new StringBuilder()
            .Append($"{this.Id}, ")
            .Append($"{this.FirstName}, ")
            .Append($"{this.LastName}, ")
            .Append($"{this.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture)}, ")
            .Append($"{this.Bonuses}, ")
            .Append($"{this.Salary}, ")
            .Append($"{this.AccountType}").ToString();
    }
}