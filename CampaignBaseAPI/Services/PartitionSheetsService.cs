using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using CampaignBaseAPI.Services.Interface;
using static CampaignBaseAPI.Enums.SheetsSize;
using CampaignBaseAPI.Constants;

namespace CampaignBaseAPI.Services
{
    public class PartitionSheetsService : IPartitionSheetsService
    {
        public async Task<Dictionary<string, MemoryStream>> PartitionSheetsAsync(MemoryStream file, Sizes size, string fileName)
        {
            file.Position = 0; // reseta a posição do stream
            using var reader = new StreamReader(file);

            var count = default(int);
            int fileNamedNumberPage = 1;
            var partitionedSheets = new Dictionary<string, MemoryStream>();
            var header = await reader.ReadLineAsync(); // Lê o cabeçalho do arquivo CSV
            var line = await reader.ReadLineAsync(); // Lê a primeira linha de dados do arquivo CSV
            var atualFile = new StringBuilder();

            if (line == null || string.IsNullOrWhiteSpace(line))
                throw new ArgumentException(ReturnMessages.EmptyFile);

            do 
            {
                if (count == 0)
                    atualFile.AppendLine(header); // Adiciona o cabeçalho ao novo arquivo

                atualFile.AppendLine(line); // Adiciona a linha atual ao novo arquivo
                count++;

                if (count >= (int)size)
                {
                    partitionedSheets.Add(GetNamePageFile(fileName, fileNamedNumberPage), new MemoryStream(Encoding.UTF8.GetBytes(atualFile.ToString())));
                    atualFile.Clear(); // Limpa o conteúdo para a próxima partição
                    count = 0; // Reseta o contador para a próxima partição
                    fileNamedNumberPage++;
                }
            } while ((line = await reader.ReadLineAsync()) != null);

            if (atualFile.Length > 0)
                partitionedSheets.Add(GetNamePageFile(fileName, fileNamedNumberPage), new MemoryStream(Encoding.UTF8.GetBytes(atualFile.ToString())));

            return partitionedSheets;
        }

        private static string GetNamePageFile(string fileName, int numberPage)
        {
            var getFileName = Path.GetFileNameWithoutExtension(fileName) ?? "Arquivo";
            return $"{getFileName}{string.Format(Constants.FileConstants.NamedFilePartition, numberPage)}{Constants.FileConstants.CsvType}";
        }
    }
}