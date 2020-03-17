using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    public class DefaultValidator : IRecordValidator
    {
        public void ValidateParameter(FileCabinetRecord record)
        {
            if (record.FirstName is null)
            {
                throw new ArgumentNullException($"Parameter {nameof(record.FirstName)} cannot be null");
            }

            if (record.LastName is null)
            {
                throw new ArgumentNullException($"Parameter {nameof(record.LastName)} cannot be null");
            }

            if (record.FirstName.Length < 2 || record.FirstName.Length > 60)
            {
                throw new ArgumentException($"Parameter {nameof(record.FirstName)} cannot have length less than 3 or more than 60");
            }

            if (record.LastName.Length < 2 || record.LastName.Length > 60)
            {
                throw new ArgumentException($"Parameter {nameof(record.LastName)} cannot have length less than 3 or more than 60");
            }

            if (record.FirstName.Trim().Length == 0)
            {
                throw new ArgumentException($"Parameter {nameof(record.FirstName)} cannot have only whitespaces");
            }

            if (record.LastName.Trim().Length == 0)
            {
                throw new ArgumentException($"Parameter {nameof(record.LastName)} cannot have only whitespaces");
            }

            if (record.DateOfBirth.Day < new DateTime(1950, 01, 01).Day || record.DateOfBirth.Day > DateTime.Now.Day)
            {
                throw new ArgumentException($"Parameter {nameof(record.DateOfBirth)} cannot be less than 01-Jan-1950 and more than current day");
            }
        }

    }
}
