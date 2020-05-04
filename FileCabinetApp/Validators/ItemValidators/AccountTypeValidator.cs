using System;

namespace FileCabinetApp
{
    public class AccountTypeValidator : IRecordValidator
    {
        public void ValidateParameters(ValidateParametersData data)
        {
            if (data is null)
            {
                throw new ArgumentNullException($"{nameof(data)} cannot be null.");
            }

            if (!char.IsLetter(data.AccountType))
            {
                throw new ArgumentException($"{nameof(data.AccountType)} should be a letter.");
            }
        }
    }
}
