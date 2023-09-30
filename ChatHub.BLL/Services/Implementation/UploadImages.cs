using ChatHub.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatHub.BLL.Services.Implementation
{
    public class UploadImages : IUploadImages
    {
        private readonly IWebHostEnvironment _environment;

        public UploadImages(IWebHostEnvironment enviroment)
        {
            _environment = enviroment;   
        }
        public void DeleteImageFromFolder(string URL)
        {
            var filename = Path.GetFileName(URL);
            var path = _environment.WebRootPath + "\\Images\\" + filename;
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            var special = Guid.NewGuid().ToString();
            if (!Directory.Exists(_environment.WebRootPath + "\\Images"))
            {
                Directory.CreateDirectory(_environment.WebRootPath + "\\Images\\");
            }
            var filePath = _environment.WebRootPath + "\\Images\\" + file.FileName;
            using (FileStream sm = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(sm);
            }
            var fileName = file.FileName;

            filePath = "Images\\" + fileName;
            return filePath;
        }

        public async Task<IFormFile> ConvertBase64ToIFormFile(string base64String, string fileName, string fileExtension)
        {
            try
            {
                byte[] imageBytes = Convert.FromBase64String(base64String);

                using (MemoryStream memoryStream = new MemoryStream(imageBytes))
                {
                    IFormFile imageFile = new FormFile(memoryStream, 0, memoryStream.Length, Guid.NewGuid().ToString(), fileName+'.'+fileExtension);
                    return imageFile;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
