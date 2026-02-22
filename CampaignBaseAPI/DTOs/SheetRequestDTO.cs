
using System.ComponentModel.DataAnnotations;
using static CampaignBaseAPI.Enums.SheetsSize;

namespace CampaignBaseAPI.DTOs
{
    public class SheetRequestDTO
    {
        [Required(ErrorMessage = Constants.ReturnMessages.SizedRequired)]
        public Sizes Size { get; set; } 

        [Required(ErrorMessage = Constants.ReturnMessages.FileRequired)]
        public required IFormFile File { get; set; }
    }

}