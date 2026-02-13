using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using static CampaignBaseAPI.Enums.SheetsSize;

namespace CampaignBaseAPI.Services.Interface
{
    public interface IPartitionSheetsService
    {
        /// <summary>
        /// Doing the partition of the base sent, creating a new Files for each partition and returning a dictionary with the partitioned sheets.
        /// </summary>
        /// <param name="file"> CSV file to be partitioned</param>
        /// <param name="size">The size of each partition</param>
        /// <param name="fileName">The name of the original file</param>
        /// <returns>Dictionary with the partitioned sheets, where the key is the file name and the value is the MemoryStream of the partitioned sheet</returns>
        Task<Dictionary<string, MemoryStream>> PartitionSheetsAsync(MemoryStream file, Sizes size, string fileName);
    }
}