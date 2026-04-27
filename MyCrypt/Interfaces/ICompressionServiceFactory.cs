using MyCrypt.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyCrypt.Interfaces
{
    internal interface ICompressionServiceFactory
    {
        ICompressionService Create(CompressionType algorithm);
    }
}
