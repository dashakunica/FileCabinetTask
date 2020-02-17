using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    public interface IRecordValidator
    {
        public void ValidateParameter(FileCabinetRecord value);
    }
}
