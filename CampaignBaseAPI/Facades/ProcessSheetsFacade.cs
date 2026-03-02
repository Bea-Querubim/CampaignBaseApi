using CampaignBaseAPI.DTOs;
using CampaignBaseAPI.Facades.Interface;
using CampaignBaseAPI.Services.Interface;

namespace CampaignBaseAPI.Facades
{
    public class ProcessSheetsFacade : IProcessSheetsFacade
    {
        private readonly IConverterFileService _converterFileService;
        private readonly IBaseValidationService _baseValidationService;
        private readonly IPartitionSheetsService _partitionSheetsService;
        private readonly IZipService _zipService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessSheetsFacade"/> class.
        /// </summary>
        /// <param name="converterFileService">The service responsible for converting files.</param>
        /// <param name="baseValidationService">The service responsible for validating and cleaning the base.</param>
        /// <param name="partitionSheetsService">The service responsible for partitioning sheets.</param>
        /// <param name="zipService">The service responsible for zip file operations.</param>
        public ProcessSheetsFacade(IConverterFileService converterFileService, IBaseValidationService baseValidationService, IPartitionSheetsService partitionSheetsService, IZipService zipService)
        {
            _converterFileService = converterFileService;
            _baseValidationService = baseValidationService;
            _partitionSheetsService = partitionSheetsService;
            _zipService = zipService;
        }

        public async Task<byte[]> ProcessSheetsAsync(SheetRequestDTO requestDTO)
        {
            var fileConverted = await _converterFileService.ConvertFileAsync(requestDTO.File);
            var validationResult = await _baseValidationService.ValidateAndCleanBaseAsync(fileConverted);
            
            var partionSheets = await _partitionSheetsService.PartitionSheetsAsync(validationResult.CleanedFile!, requestDTO.Size, requestDTO.File.FileName);
            
            var zipFilePath = await _zipService.CreateZipFileAsync(partionSheets, validationResult.Report, validationResult.DuplicatedPhonesNormalized!, validationResult.RemovedRowDetails!);
            return zipFilePath;
        }
    }
}