using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CampaignBaseAPI.Services.Interface
{
    public interface IPartitionSheetsService
    {
        /// <summary>
        /// Doing the partition of the base sent, creating a new Files for each partition andf returning a list of FileStreams with the partitioned sheets.
        /// </summary>
        /// <param name="file"> CSV file to be partitioned</param>
        /// <returns>List of FormFiles representing the partitioned sheets</returns>
        Task<List<IFormFile>> PartitionSheetsServices(IFormFile file);
    }
}