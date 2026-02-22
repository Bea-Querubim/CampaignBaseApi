using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CampaignBaseAPI.Models;

namespace CampaignBaseAPI.Services.Interface
{
    
    public interface IBaseValidationService
    {
        /// <summary>
        /// Validates and cleans the provided base file.
        /// </summary>
        /// <param name="file">The file to be validated and cleaned</param>
        /// <returns>A Task that represents the asynchronous operation, containing the validation result</returns>
        /// <exception cref="Exception">Throws an exception if the validation fails</exception>
        public Task<BaseValidationResult> ValidateAndCleanBaseAsync (MemoryStream file);
    }
}