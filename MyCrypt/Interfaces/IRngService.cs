using System;
using System.Collections.Generic;
using System.Text;

namespace MyCrypt.Interfaces
{
    internal interface IRngService
    {
        byte[] GenerateRandomBytes(int size);
    }
}
