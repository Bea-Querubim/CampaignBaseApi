using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CampaignBaseAPI.Services.Interface
{
    public interface IConverterFileService
    {
        /// <summary>
        /// FileStream é usado para leitura dos dados do arquivo, permitindo que o conteúdo seja processado sem a necessidade de carregar o arquivo inteiro na memória, o que é especialmente útil para arquivos grandes/medios.
        /// </summary>
        /// <param name="file">The file to be converted to .csv</param>
        /// <returns>A Task that represents the asynchronous operation, containing the converted file as a MemoryStream</returns>
        /// <exception cref="Exception">Throws an exception if the file conversion fails</exception>
        public Task<MemoryStream> ConvertFileAsync(IFormFile file);
    }
}