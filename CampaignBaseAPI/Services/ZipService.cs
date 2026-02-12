using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CampaignBaseAPI.Services.Interface;

namespace CampaignBaseAPI.Services
{
    public class ZipService : IZipService
    {
        public Task<byte[]> CreaterZipFileAsync(List<IFormFile> files)
        {
            throw new NotImplementedException();
        }
    }
}