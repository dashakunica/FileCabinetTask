using System;
using System.Globalization;
using System.Text;
using System.Xml.Serialization;

namespace FileCabinetApp
{
    /// <summary>
    /// Model for serialization record.
    /// </summary>
    [Serializable]
    public class FileCabinetRecordSerializable
    {
        /// <summary>
        /// Gets or sets id for serialization model.
        /// </summary>
        /// <value>
        /// Id.
        /// </value>
        [XmlAttribute("Id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets name for serialization model.
        /// </summary>
        /// <value>
        /// Name.
        /// </value>
        [XmlElement("name")]
        public Name Name { get; set; }

        /// <summary>
        /// Gets or sets date of birth for serialization model.
        /// </summary>
        /// <value>
        /// Date of birth.
        /// </value>
        [XmlIgnore]
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets date of birth string format.
        /// </summary>
        /// <value>
        /// Date of birth.
        /// </value>
        [XmlElement("DateOfBirth")]
        public string DateString
        {
            get { return this.DateOfBirth.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture); }
            set { this.DateOfBirth = DateTime.Parse(value, CultureInfo.InvariantCulture); }
        }

        /// <summary>
        /// Gets or sets account type for serialization model.
        /// </summary>
        /// <value>
        /// Account type.
        /// </value>
        [XmlElement("AccountType")]
        public char AccountType { get; set; }

        /// <summary>
        /// Gets or sets salary for serialization model.
        /// </summary>
        /// <value>
        /// Salary.
        /// </value>
        [XmlElement("Salary")]
        public decimal Salary { get; set; }

        /// <summary>
        /// Gets or sets bonuses for serialization model.
        /// </summary>
        /// <value>
        /// Bonuses.
        /// </value>
        [XmlElement("Bonuses")]
        public short Bonuses { get; set; }

        /// <summary>
        /// Overriding method for to string representation of serialization model.
        /// </summary>
        /// <returns>String.</returns>
        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append($"{this.Id}, ");
            builder.Append($"{this.Name?.FirstName}, ");
            builder.Append($"{this.Name?.LastName}, ");
            builder.Append($"{this.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture)}, ");
            builder.Append($"{this.Bonuses}, ");
            builder.Append($"{this.Salary}, ");
            builder.Append($"{this.AccountType}");
            return builder.ToString();
        }
    }
}
