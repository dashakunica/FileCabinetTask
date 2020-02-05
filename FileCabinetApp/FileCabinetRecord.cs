using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    public class FileCabinetRecord
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public short Popertie1 { get; set; }

        public decimal Popertie2 { get; set; }

        public char Popertie3 { get; set; }

        public override string ToString()
        {
            return string.Format("#{0}, {1}, {2}, {3:D}", this.Id, this.FirstName, this.LastName, this.DateOfBirth);
        }
    }
}
