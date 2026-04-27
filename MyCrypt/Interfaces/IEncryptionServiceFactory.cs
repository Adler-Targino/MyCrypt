using MyCrypt.Models;

namespace MyCrypt.Interfaces
{
    internal interface IEncryptionServiceFactory
    {
        IEncryptionService Create(EncryptionType algorithm);
    }
}
