using MyCrypt.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace MyCrypt.Services
{
    internal class RngService : IRngService
    {
        public byte[] GenerateRandomBytes(int size)
        {
            byte[] randomBytes = RandomNumberGenerator.GetBytes(size);
            return randomBytes;
        }
    }
}
