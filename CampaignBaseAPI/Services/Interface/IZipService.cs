using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace CampaignBaseAPI.Services.Interface
{
    public interface IZipService
    {
        /// <summary>
        /// Creating a zip file with the partitioned sheets.
        /// </summary>
        /// <param name="files">Dictionary containing file names and their corresponding MemoryStream objects</param>
        /// <returns>Byte array representing the zipped file</returns>
        Task<byte[]> CreateZipFileAsync(Dictionary<string, MemoryStream> files);
    }
}