using System;
using System.Globalization;

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
        /// Gets or sets Propertie1 of record.
        /// </summary>
        /// <value>
        /// Popertie1.
        /// </value>
        public short Status { get; set; }

        /// <summary>
        /// Gets or sets Propertie2 of record.
        /// </summary>
        /// <value>
        /// Propertie2.
        /// </value>
        public decimal Propertie2 { get; set; }

        /// <summary>
        /// Gets or sets Propertie3 of record.
        /// </summary>
        /// <value>
        /// Propertie3.
        /// </value>
        public char Propertie3 { get; set; }

        /// <summary>
        /// Override method for string representation of record.
        /// </summary>
        /// <returns>String representation of record.</returns>
        public override string ToString()
        {
            return string.Format(new CultureInfo("en-US"), "#{0}, {1}, {2}, {3}", this.Id, this.FirstName, this.LastName, this.DateOfBirth);
        }
    }
}
