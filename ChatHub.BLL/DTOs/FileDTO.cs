using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatHub.BLL.DTOs
{
    public class FileDTO
    {
        public IFormFile? File { get; set; }
        public string? SenderUsername { get; set; }
        public string? Receiverusername { get; set; }
    }
}
