using System;

namespace FileCabinetApp
{
    public interface IInputValidator
    {
        Tuple<bool, string> ValidateFirstName(string firstName);

        Tuple<bool, string> ValidateLastName(string lastName);

        Tuple<bool, string> ValidateDateOfBirth(DateTime dateOfBirth);

        Tuple<bool, string> ValidateSex(char sex);

        Tuple<bool, string> ValidateWeight(decimal weight);

        Tuple<bool, string> ValidateHeight(short height);
    }
}
