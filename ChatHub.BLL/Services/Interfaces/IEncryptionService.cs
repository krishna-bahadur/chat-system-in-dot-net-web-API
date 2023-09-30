using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatHub.BLL.Services.Interfaces
{
    public interface IEncryptionService
    {
        Task<string> Encrypt(string message);
        Task<string> Decrypt(string chiphertext);
    }
}
