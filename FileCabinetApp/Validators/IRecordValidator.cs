using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    public interface IRecordValidator
    {
        public void ValidateParameter(FileCabinetRecord value);

        Tuple<bool, string> ValidateFirstName(string firstName);

        Tuple<bool, string> ValidateLastName(string lastName);

        Tuple<bool, string> ValidateDateOfBirth(DateTime dateOfBirth);

        Tuple<bool, string> ValidateAccountType(char accoountType);

        Tuple<bool, string> ValidateSalary(decimal salary);

        Tuple<bool, string> ValidateBonuses(short bonuses);
    }
}
