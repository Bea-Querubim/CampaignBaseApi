using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using static CampaignBaseAPI.Enums.SheetsSize;
using System.IO;
using System.Text;
using CampaignBaseAPI.Services;

namespace CampaignBaseAPI.Tests.Services
{
    public class PartitionSheetsServiceTests
    {
        public PartitionSheetsServiceTests()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        [Fact]
        public async Task PartitionSheetsAsync_ShouldReturnDictionaryWithFiles_WhenPartitionIsSuccessful()
        {
            // Arrange
            PartitionSheetsService partitionSheetsService = new PartitionSheetsService();
            var fileStreamMock = new MemoryStream(Encoding.UTF8.GetBytes(GenerateCvsMock(3000))); // Gerar um CSV com 2000 linhas

            // Act
            var sheetsFile = await partitionSheetsService.PartitionSheetsAsync(fileStreamMock, Sizes.dois_mil, "file.csv");

            // Assert
            Assert.Equal(2, sheetsFile.Count); // Espera-se 2 arquivos: um com 2000 linhas e outro com 1000 linhas

                //valida os nomes
            Assert.Contains("file_PAG-1.csv", sheetsFile.Keys);
            Assert.Contains("file_PAG-2.csv", sheetsFile.Keys);

                //verifica o conteudo
            sheetsFile["file_PAG-1.csv"].Position = 0; // ← Reset
            Assert.Equal(2000, CountLines(sheetsFile["file_PAG-1.csv"])); // Verifica se o primeiro arquivo tem 2000 linhas
            sheetsFile["file_PAG-1.csv"].Position = 0; // ← Reset
            Assert.Contains("phone,link,name", new StreamReader(sheetsFile["file_PAG-1.csv"]).ReadLine()); // Verifica se o cabeçalho está presente no primeiro arquivo  

            sheetsFile["file_PAG-2.csv"].Position = 0; // ← Reset
            Assert.Equal(1000, CountLines(sheetsFile["file_PAG-2.csv"])); // Verifica se o segundo arquivo tem 1000 linhas
            sheetsFile["file_PAG-2.csv"].Position = 0; // ← Reset
            Assert.Contains("phone,link,name", new StreamReader(sheetsFile["file_PAG-2.csv"]).ReadLine()); // Verifica se o cabeçalho está presente no segundo arquivo
        }

        [Fact]
        public async Task PartitionSheetsAsync_ShouldReturnOneFile_WhenDataLinesAreExactlyTheSize()
        {  
            // Arrange
            PartitionSheetsService partitionSheetsService = new PartitionSheetsService();
            var fileStreamMock = new MemoryStream(Encoding.UTF8.GetBytes(GenerateCvsMock(2000))); // Gerar um CSV com 2000 linhas

            // Act
            var sheetsFile = await partitionSheetsService.PartitionSheetsAsync(fileStreamMock, Sizes.dois_mil, "file.csv");

            // Assert
            Assert.Single(sheetsFile); // Espera-se 1 arquivo: com 2000 linhas
                //valida os nomes
            Assert.Contains("file_PAG-1.csv", sheetsFile.Keys);
        }

        [Fact]
        public async Task PartitionSheetsAsync_ShouldReturnMultipleFiles_WhenDataExceedsSize()
        {  
            // Arrange
            PartitionSheetsService partitionSheetsService = new PartitionSheetsService();
            var fileStreamMock = new MemoryStream(Encoding.UTF8.GetBytes(GenerateCvsMock(5500))); // Gerar um CSV com 5500 linhas

            // Act
            var sheetsFile = await partitionSheetsService.PartitionSheetsAsync(fileStreamMock, Sizes.dois_mil, "file.csv");

            // Assert
            Assert.Equal(3, sheetsFile.Count); // Espera-se 3 arquivos: dois com 2000 linhas e um com 1500 linhas
                //valida os nomes
            Assert.Contains("file_PAG-1.csv", sheetsFile.Keys);
            Assert.Contains("file_PAG-2.csv", sheetsFile.Keys);
            Assert.Contains("file_PAG-3.csv", sheetsFile.Keys); 
        }

        [Fact]
        public async Task PartitionSheetsAsync_ShouldThrowException_WhenFileHasOnlyHeader()
        {
            // Arrange
            PartitionSheetsService partitionSheetsService = new PartitionSheetsService();
            var fileStreamMock = new MemoryStream(Encoding.UTF8.GetBytes("phone,link,name\n")); // Gerar um CSV com apenas o cabeçalho

            // Act & Assert
           await Assert.ThrowsAsync<ArgumentException>(() => partitionSheetsService.PartitionSheetsAsync(fileStreamMock, Sizes.um_mil, "file.csv"));
        }

        private string GenerateCvsMock(int numberOfLines)
        {
            var csvContent = new StringBuilder();
            csvContent.AppendLine("phone,link,name"); // Cabeçalho do CSV

            for (int i = 0; i < numberOfLines; i++)
            {
                csvContent.AppendLine($"1198888{i:D4},https://url{i}.com,Person{i}");
            }

            return csvContent.ToString();
        }

        private int CountLines(MemoryStream stream)
        {
            stream.Position = 0; // Reseta a posição do stream para o início
            using var reader = new StreamReader(stream, leaveOpen: true); // Deixa o stream aberto para que possa ser lido posteriormente
            var count = 0;
            var line = default(string);

            reader.ReadLine(); // ← Lê o cabeçalho (mas não conta!)
            while ((line = reader.ReadLine()) != null)
            {
                count++;
            }
            return count;
        }
    }

}