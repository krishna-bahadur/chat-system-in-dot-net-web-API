using ChatHub.BLL.Services.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ChatHub.BLL.Services.Implementation
{
    public class EncryptionService : IEncryptionService
    {
        private readonly IConfiguration _configuration;
        public EncryptionService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<string> Decrypt(string chiphertext)
        {
            try
            {
                byte[] encryptedMessageFromDatabase = Convert.FromBase64String(chiphertext);

                string decryptedMessage;

                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = Convert.FromBase64String(_configuration["key"]);
                    aesAlg.IV = Convert.FromBase64String(_configuration["iv"]);

                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    using (var msDecrypt = new System.IO.MemoryStream(encryptedMessageFromDatabase))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (var srDecrypt = new System.IO.StreamReader(csDecrypt))
                            {
                                decryptedMessage = srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }
                return decryptedMessage;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<string> Encrypt(string message)
        {
            try
            {
                byte[] encryptedMessage;
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = Convert.FromBase64String(_configuration["key"]); 
                    aesAlg.IV = Convert.FromBase64String(_configuration["iv"]);   

                    ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                    using (var msEncrypt = new System.IO.MemoryStream())
                    {
                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (var swEncrypt = new System.IO.StreamWriter(csEncrypt))
                            {
                                swEncrypt.Write(message);
                            }
                        }
                        encryptedMessage = msEncrypt.ToArray();
                    }
                }
                return Convert.ToBase64String(encryptedMessage);

            }
            catch(Exception ex)
            {
                throw;
            }
        }
    }
}
