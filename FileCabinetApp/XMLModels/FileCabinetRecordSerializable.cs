using System;
using System.Globalization;
using System.Text;
using System.Xml.Serialization;

namespace FileCabinetGenerator
{
    [Serializable]
    public class FileCabinetRecordSerializable
    {
        [XmlAttribute("id")]
        public int Id { get; set; }

        [XmlElement("name")]
        public Name Name = new Name();

        [XmlIgnore]
        public DateTime DateOfBirth { get; set; }

        [XmlElement("DateOfBirth")]
        public string SomeDateString
        {
            get { return this.DateOfBirth.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture); }
            set { this.DateOfBirth = DateTime.Parse(value, CultureInfo.InvariantCulture); }
        }

        [XmlElement("AccountType")]
        public char AccountType { get; set; }

        [XmlElement("Salary")]
        public decimal Salary { get; set; }

        [XmlElement("Bonuses")]
        public short Bonuses { get; set; }

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

    public class Name
    {
        [XmlAttribute("first")]
        public string FirstName { get; set; }

        [XmlAttribute("last")]
        public string LastName { get; set; }
    }
}
