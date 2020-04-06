using System;

namespace FileCabinetApp
{
    public class AccountTypeValidator : IRecordValidator
    {
        public void ValidateParameters(FileCabinetRecord data)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data), $"{nameof(data)} cannot be null.");
            }

            if (!char.IsLetter(data.AccountType))
            {
                throw new ArgumentException(nameof(data.AccountType), $"{nameof(data.AccountType)} should be a letter.");
            }
        }
    }
}
