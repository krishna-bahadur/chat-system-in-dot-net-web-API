using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatHub.BLL.DTOs
{
    public class EncryptedCipher
    {
        public string? aes_key { get; set; }
        public string? cipher_text { get; set; }
        public string? iv { get; set; }
    }
}
