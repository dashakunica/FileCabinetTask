using System;
using System.Collections.Generic;

namespace FileCabinetApp
{
    public interface IFileCabinetReader : IDisposable
    {
        IList<FileCabinetRecord> ReadAll();
    }
}
