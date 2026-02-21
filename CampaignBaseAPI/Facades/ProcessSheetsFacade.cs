using CampaignBaseAPI.DTOs;
using CampaignBaseAPI.Facades.Interface;
using CampaignBaseAPI.Services.Interface;

namespace CampaignBaseAPI.Facades
{
    public class ProcessSheetsFacade : IProcessSheetsFacade
    {
        private readonly IConverterFileService _converterFileService;
        private readonly IPartitionSheetsService _partitionSheetsService;
        private readonly IZipService _zipService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessSheetsFacade"/> class.
        /// </summary>
        /// <param name="converterFileService">The service responsible for converting files.</param>
        /// <param name="partitionSheetsService">The service responsible for partitioning sheets.</param>
        /// <param name="zipService">The service responsible for zip file operations.</param>
        public ProcessSheetsFacade(IConverterFileService converterFileService, IPartitionSheetsService partitionSheetsService, IZipService zipService)
        {
            _converterFileService = converterFileService;
            _partitionSheetsService = partitionSheetsService;
            _zipService = zipService;
        }

        //return a byte, 'cause the file will be downloaded in swagger, so we need to return a byte array to be able to download the file.
        public async Task<byte[]> ProcessSheetsAsync(SheetRequestDTO requestDTO)
        {
            //[service]
            // primeiro: verifica se o arquivo esta vazio ou nullo, independente da extensao
            // verifica extensao do arquivo, se for csv, nao precisa converter mas transforma em MemoryStream, se for xls ou xlsx, converter para csv <MemoryStream>
            var fileConverted = await _converterFileService.ConvertFileAsync(requestDTO.File);
            
            // ler o arquivo e fazer a partição da base enviada
            var partionSheets = await _partitionSheetsService.PartitionSheetsAsync(fileConverted, requestDTO.Size, requestDTO.File.FileName);
            
            //criar a pasta e salvar os aquivos particionados
            var zipFilePath = await _zipService.CreateZipFileAsync(partionSheets);

            //retornar resposta para o controller
            return zipFilePath;
        }
    }
}