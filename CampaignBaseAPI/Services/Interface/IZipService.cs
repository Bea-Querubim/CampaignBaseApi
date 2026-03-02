using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CampaignBaseAPI.Models;

namespace CampaignBaseAPI.Services.Interface
{
    public interface IZipService
    {
        /// <summary>
        /// Creating a zip file with the partitioned sheets.
        /// </summary>
        /// <param name="files">Dictionary containing file names and their corresponding MemoryStream objects</param>
        /// <param name="report">String containing the report summary</param>
        /// <param name="duplicatedPhones">List of duplicated phone numbers</param>
        /// <param name="removedRowDetails">List of details for removed rows</param>
        /// <returns>Byte array representing the zipped file</returns>
        Task<byte[]> CreateZipFileAsync(Dictionary<string, MemoryStream> files, string report, List<string> duplicatedPhones, List<RemovedRowDetail> removedRowDetails);
    }
}