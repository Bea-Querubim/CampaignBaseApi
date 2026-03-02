
using System.ComponentModel.DataAnnotations;
using static CampaignBaseAPI.Enums.SheetsSize;

namespace CampaignBaseAPI.DTOs
{
    /// <summary>
    /// Request payload for sheet processing.
    /// </summary>
    public class SheetRequestDTO
    {
        /// <summary>
        /// Target chunk size for output partitions.
        /// </summary>
        [Required(ErrorMessage = Constants.ReturnMessages.SizedRequired)]
        public Sizes Size { get; set; } 

        /// <summary>
        /// Input file (.csv, .xls, or .xlsx) uploaded as multipart form-data.
        /// </summary>
        [Required(ErrorMessage = Constants.ReturnMessages.FileRequired)]
        public required IFormFile File { get; set; }
    }

}