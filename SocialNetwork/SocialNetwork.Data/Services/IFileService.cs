using Microsoft.AspNetCore.Http;
using SocialNetwork.Data.Helpers.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.Data.Services
{
    public interface IFilesService
    {
        Task<string> UploadImageAsync(IFormFile file, ImageFileType imageFileType);
    }
}
