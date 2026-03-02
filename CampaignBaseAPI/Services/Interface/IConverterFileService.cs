using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CampaignBaseAPI.Services.Interface
{
    public interface IConverterFileService
    {
        /// <summary>
        /// Converts an uploaded file to CSV content in memory.
        /// </summary>
        /// <param name="file">The file to be converted to .csv</param>
        /// <returns>The converted file content as a <see cref="MemoryStream"/>.</returns>
        public Task<MemoryStream> ConvertFileAsync(IFormFile file);
    }
}