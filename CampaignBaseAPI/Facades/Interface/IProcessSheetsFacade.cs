using CampaignBaseAPI.DTOs;

namespace CampaignBaseAPI.Facades.Interface
{
    public interface IProcessSheetsFacade
    {
        /// <summary>
        /// Processes a sheet based on the provided request DTO.
        /// </summary>
        /// <param name="requestDTO">The request DTO containing sheet data to be processed</param>
        public Task<byte[]> ProcessSheetsAsync( SheetRequestDTO requestDTO);
    }
}