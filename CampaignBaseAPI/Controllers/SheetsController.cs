
using CampaignBaseAPI.Constants;
using CampaignBaseAPI.DTOs;
using CampaignBaseAPI.Facades.Interface;
using Microsoft.AspNetCore.Mvc;

namespace CampaignBaseAPI.Controllers
{
    /// <summary>
    /// Endpoints for campaign sheet processing.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SheetsController : ControllerBase
    {
        private readonly IProcessSheetsFacade _processSheetsFacade;

        /// <summary>
        /// Initializes a new instance of the <see cref="SheetsController"/> class.
        /// </summary>
        /// <param name="processSheetsFacade">Facade that orchestrates conversion, validation, partitioning and zip creation.</param>
        public SheetsController(IProcessSheetsFacade processSheetsFacade)
        {
            _processSheetsFacade = processSheetsFacade;
        }

        /// <summary>
        /// Processes an uploaded sheet and returns the generated ZIP file.
        /// </summary>
        /// <param name="requestDTO">Multipart form-data request containing the source file and desired partition size.</param>
        /// <returns>A ZIP file containing partitioned CSV files and processing report artifacts.</returns>
        /// <response code="200">The sheet was processed successfully and a ZIP file is returned.</response>
        /// <response code="400">The request is invalid (empty file or unsupported extension).</response>
        /// <response code="500">An unexpected error occurred during processing.</response>
        [Consumes("multipart/form-data")]
        [Produces("application/zip")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
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