
using CampaignBaseAPI.Constants;
using CampaignBaseAPI.DTOs;
using CampaignBaseAPI.Facades.Interface;
using Microsoft.AspNetCore.Mvc;

namespace CampaignBaseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SheetsController : ControllerBase
    {
        private readonly IProcessSheetsFacade _processSheetsFacade;
        public SheetsController(IProcessSheetsFacade processSheetsFacade)
        {
            _processSheetsFacade = processSheetsFacade;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("process")]
        public async Task<IActionResult> ProcessSheets([FromForm] SheetRequestDTO requestDTO)
        {
            if (requestDTO.File.Length <= 0)
            {
                return BadRequest(ReturnMessages.EmptyFile);
            }

            var fileExtension = Path.GetExtension(requestDTO.File.FileName).ToLower();
            if (!FileConstants.AllowedExtensions.Contains(fileExtension))
                return BadRequest(string.Format(ReturnMessages.InvalidFileExtension, string.Join(", ", FileConstants.AllowedExtensions)));

            var file = await _processSheetsFacade.ProcessSheetsAsync(requestDTO);

            return File(file, "application/zip", $"{Path.GetFileNameWithoutExtension(requestDTO.File.FileName)}.zip");
        }
    }
}