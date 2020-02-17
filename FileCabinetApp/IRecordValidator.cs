using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    public interface IRecordValidator<T>
    {
        public static void ValidateParameter(T value);
    }
}
