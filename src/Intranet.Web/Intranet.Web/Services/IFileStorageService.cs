﻿using Intranet.Shared.Common.Enums;
using Microsoft.AspNetCore.Http;

namespace Intranet.Web.Services
{
    public interface IFileStorageService
    {
        (string path, string mime) GetFile(string filename);
        (string path, string mime) GetImage(string filename, ImageVariantType imageVariantType = ImageVariantType.Original);
        string SaveBlob(string filePath, IFormFile file);
        string SaveImage(string filePath, IFormFile image);
    }
}