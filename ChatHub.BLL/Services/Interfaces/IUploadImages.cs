using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatHub.BLL.Services.Interfaces
{
    public interface IUploadImages
    {
        Task<string> UploadImageAsync(IFormFile file);
        void DeleteImageFromFolder(string URL);
        Task<IFormFile> ConvertBase64ToIFormFile(string base64String, string fileName, string fileExtension);
    }
}
