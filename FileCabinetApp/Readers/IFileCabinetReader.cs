﻿using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    public interface IFileCabinetReader
    {
        IList<FileCabinetRecord> ReadAll();
    }
}