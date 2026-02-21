using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using CampaignBaseAPI.Constants;
using CampaignBaseAPI.Services;

namespace CampaignBaseAPI.Tests.Services
{
    public class ZipServiceTests
    {
        [Fact]
        public async Task CreateZipFileAsync_ShouldThrowArgumentNullException_WhenFilesIsNull()
        {
            // Arrange
            ZipService zipService = new ZipService();
            Dictionary<string, MemoryStream>? zipNullable = null;

            // Act
            // Assert.ThrowsAsync é usado para verificar se a chamada do método CreateZipFileAsync lança uma exceção do tipo ArgumentNullException quando o dicionário de arquivos é nulo. O teste passa se a exceção for lançada, indicando que o método está lidando corretamente com a entrada nula.
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => zipService.CreateZipFileAsync(zipNullable!));
            // Assert
            Assert.Equal("files", exception.ParamName);
        }

        [Fact]
        public async Task CreateZipFileAsync_ShouldThrowArgumentException_WhenFilesIsEmpty()
        {
            // Arrange
            ZipService zipService = new ZipService();
            var files = new Dictionary<string, MemoryStream>();

            // Act & Assert
            // Assert.ThrowsAsync é usado para verificar se a chamada do método CreateZipFileAsync lança uma exceção do tipo ArgumentException quando o dicionário de arquivos está vazio. O teste passa se a exceção for lançada, indicando que o método está lidando corretamente com a entrada vazia.
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => zipService.CreateZipFileAsync(files));
            Assert.Equal("files", exception.ParamName);
        }

        [Fact]
        public async Task CreateZipFileAsync_ShouldCreateZipWithExpectedEntries_WhenAllEntriesHaveFullContent()
        {
            // Arrange
            ZipService zipService = new ZipService();
            var files = new Dictionary<string, MemoryStream>
            {
                ["file_PAG-1.csv"] = CreateCsvStream(GenerateDataMock(1000)) 
            };

            // Act
            var zipBytes = await zipService.CreateZipFileAsync(files);

            // Assert
            Assert.NotNull(zipBytes);
            Assert.True(zipBytes.Length > 0);

            using var zipStream = new MemoryStream(zipBytes);
            using var zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Read);

            Assert.Single(zipArchive.Entries);
            Assert.Equal("file_PAG-1.csv", zipArchive.Entries[0].FullName);

            using var entryStream = zipArchive.Entries[0].Open();
            using var reader = new StreamReader(entryStream);
            var entryContent = await reader.ReadToEndAsync();

            Assert.Equal(1000, CountLines(entryContent));
        }

        [Fact]
        public async Task CreateZipFileAsync_ShouldReturnZipWithMultipleEntries_WhenLastOfMultipleFilesArePartialContent()
        {
            // Arrange
            ZipService zipService = new ZipService();
            var files = new Dictionary<string, MemoryStream>
            {
                ["file_PAG-1.csv"] = CreateCsvStream(GenerateDataMock(1000)),
                ["file_PAG-2.csv"] = CreateCsvStream(GenerateDataMock(200))
            };

            // Act
            var zipBytes = await zipService.CreateZipFileAsync(files);

            // Assert
            Assert.NotNull(zipBytes);
            Assert.True(zipBytes.Length > 0);

            using var zipStream = new MemoryStream(zipBytes);
            using var zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Read);

            Assert.Equal(2, zipArchive.Entries.Count);
            Assert.Contains(zipArchive.Entries, entry => entry.FullName == "file_PAG-1.csv");
            Assert.Contains(zipArchive.Entries, entry => entry.FullName == "file_PAG-2.csv");

            using var entryStream = zipArchive.Entries.First(entry => entry.FullName == "file_PAG-2.csv").Open();
            using var reader = new StreamReader(entryStream);
            var entryContent = await reader.ReadToEndAsync();

            Assert.Equal(200, CountLines(entryContent));
        }

        [Fact]
        public async Task CreateZipFileAsync_ShouldThrowZipMontageException_WhenErrorOccursDuringZipCreation()
        {
            // Arrange
            ZipService zipService = new ZipService();
            var disposedStream = CreateCsvStream("phone,link,name\n11988888888,https://url1.com,Person1\n");
            disposedStream.Dispose();

            var files = new Dictionary<string, MemoryStream>
            {
                ["file_PAG-1.csv"] = disposedStream
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => zipService.CreateZipFileAsync(files));
            Assert.Equal(ReturnMessages.ZipErrorMontage, exception.Message);
            Assert.NotNull(exception.InnerException);
        }

        private static MemoryStream CreateCsvStream(string content)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(content));
        }

         private string GenerateDataMock(int numberOfLines)
        {
            var csvContent = new StringBuilder();
            csvContent.AppendLine("phone,link,name"); // Cabeçalho do CSV

            for (int i = 0; i < numberOfLines; i++)
            {
                csvContent.AppendLine($"1198888{i:D4},https://url{i}.com,Person{i}");
            }

            return csvContent.ToString();
        }

        private int CountLines(string content)
        {
            using var reader = new StringReader(content);
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