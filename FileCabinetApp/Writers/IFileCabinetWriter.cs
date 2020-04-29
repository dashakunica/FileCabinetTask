using System;

namespace FileCabinetApp
{
    public interface IFileCabinetWriter : IDisposable
    {
        void Write(FileCabinetRecord record);
    }
}
