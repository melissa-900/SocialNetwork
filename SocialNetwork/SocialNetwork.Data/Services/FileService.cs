using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using SocialNetwork.Data.Helpers.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.Data.Services
{
    public class FileService : IFilesService
    {
        private readonly BlobServiceClient _blobServiceClient;
        public FileService(string connectionString)
        {
            _blobServiceClient = new BlobServiceClient(connectionString);
        }
        public async Task<string> UploadImageAsync(IFormFile file, ImageFileType imageFileType)
        {
            string filePathUpload = imageFileType switch
            {
                ImageFileType.PostImage => Path.Combine("images", "posts"),
                _ => throw new ArgumentException("Invalid file type")
            };

            if (file != null && file.Length > 0)
            {
                string rootFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                if (file.ContentType.Contains("image"))
                {
                    string rootFolderPathImages = Path.Combine(rootFolderPath, filePathUpload);
                    Directory.CreateDirectory(rootFolderPathImages);

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string filePath = Path.Combine(rootFolderPathImages, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                        await file.CopyToAsync(stream);

                    //Set the URL to the newPost object

                    return $"{filePathUpload}\\{fileName}";
                }
            }

            return "";
        }
    }
}

