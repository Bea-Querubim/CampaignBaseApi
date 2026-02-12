using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CampaignBaseAPI.Services.Interface
{
    public interface IZipService
    {
        /// <summary>
        /// Creating a zip file with the partitioned sheets.
        /// </summary>
        /// <param name="file"> CSV file to be zipped</param>
        /// <returns>Byte array representing the zipped file</returns>
        Task<byte[]> CreaterZipFileAsync(List<IFormFile> files);
    }
}