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

            if (file == null || file.Length == 0)
                throw new ArgumentException(ReturnMessages.EmptyFile);

            // Garante que a posição do stream está no início
            file.Position = 0;

            using var reader = new StreamReader(file);
            var count = default(int);
            int fileNamedNumberPage = 1;
            var partitionedSheets = new Dictionary<string, MemoryStream>();
            var header = await reader.ReadLineAsync();
            var line = await reader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(header) || string.IsNullOrWhiteSpace(line))
                throw new ArgumentException(ReturnMessages.EmptyFile);
            var atualFile = new StringBuilder();

            do 
            {
                if (count == 0)
                    atualFile.AppendLine(header);

                atualFile.AppendLine(line);
                count++;

                if (count >= (int)size)
                {
                    partitionedSheets.Add(GetNamePageFile(fileName, fileNamedNumberPage), new MemoryStream(Encoding.UTF8.GetBytes(atualFile.ToString())));
                    atualFile.Clear();
                    count = 0;
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