using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using CampaignBaseAPI.Constants;
using CampaignBaseAPI.Services;
using CampaignBaseAPI.Models;

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

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => zipService.CreateZipFileAsync(zipNullable!, "report", new List<string>(), new List<RemovedRowDetail>()));
            Assert.Equal("files", exception.ParamName);
        }

        [Fact]
        public async Task CreateZipFileAsync_ShouldThrowArgumentException_WhenFilesIsEmpty()
        {
            // Arrange
            ZipService zipService = new ZipService();
            var files = new Dictionary<string, MemoryStream>();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => zipService.CreateZipFileAsync(files, "report", new List<string>(), new List<RemovedRowDetail>()));
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
            string report = "Relatório de teste";
            var duplicatedPhones = new List<string>();
            var removedRowDetails = new List<RemovedRowDetail>();

            // Act
            var zipBytes = await zipService.CreateZipFileAsync(files, report, duplicatedPhones, removedRowDetails);

            // Assert
            Assert.NotNull(zipBytes);
            Assert.True(zipBytes.Length > 0);

            using var zipStream = new MemoryStream(zipBytes);
            using var zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Read);

            // Deve conter 2 arquivos: o CSV e o Report.txt
            Assert.Equal(2, zipArchive.Entries.Count);
            Assert.Contains(zipArchive.Entries, entry => entry.FullName == "file_PAG-1.csv");
            Assert.Contains(zipArchive.Entries, entry => entry.FullName == "Report.txt");
            Assert.DoesNotContain(zipArchive.Entries, entry => entry.FullName == "DuplicatedPhones.txt");

            using var entryStream = zipArchive.Entries.First(entry => entry.FullName == "file_PAG-1.csv").Open();
            using var reader = new StreamReader(entryStream);
            var entryContent = await reader.ReadToEndAsync();
            Assert.Equal(1000, CountLines(entryContent));

            // Verifica conteúdo do Report.txt
            using var reportStream = zipArchive.Entries.First(entry => entry.FullName == "Report.txt").Open();
            using var reportReader = new StreamReader(reportStream);
            var reportContent = await reportReader.ReadToEndAsync();
            Assert.Contains("Relatório de teste", reportContent);
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
            string report = "Report content";
            var duplicatedPhones = new List<string> { "11988888888" };
            var removedRowDetails = new List<RemovedRowDetail> { new RemovedRowDetail { RowNumber = 1, OriginalPhone = "11988888888", NormalizedPhone = "11988888888", Reason = CampaignBaseAPI.Enums.RemovalReason.Reasons.DuplicatedPhone } };

            // Act
            var zipBytes = await zipService.CreateZipFileAsync(files, report, duplicatedPhones, removedRowDetails);

            // Assert
            Assert.NotNull(zipBytes);
            Assert.True(zipBytes.Length > 0);

            using var zipStream = new MemoryStream(zipBytes);
            using var zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Read);

            // Deve conter 4 arquivos: 2 CSVs, Report.txt e DuplicatedPhones.txt
            Assert.Equal(4, zipArchive.Entries.Count);
            Assert.Contains(zipArchive.Entries, entry => entry.FullName == "file_PAG-1.csv");
            Assert.Contains(zipArchive.Entries, entry => entry.FullName == "file_PAG-2.csv");
            Assert.Contains(zipArchive.Entries, entry => entry.FullName == "Report.txt");
            Assert.Contains(zipArchive.Entries, entry => entry.FullName == "DuplicatedPhones.txt");

            using var entryStream = zipArchive.Entries.First(entry => entry.FullName == "file_PAG-2.csv").Open();
            using var reader = new StreamReader(entryStream);
            var entryContent = await reader.ReadToEndAsync();
            Assert.Equal(200, CountLines(entryContent));

            // Verifica conteúdo do Report.txt
            using var reportStream = zipArchive.Entries.First(entry => entry.FullName == "Report.txt").Open();
            using var reportReader = new StreamReader(reportStream);
            var reportContent = await reportReader.ReadToEndAsync();
            Assert.Contains("Report content", reportContent);

            // Verifica conteúdo do DuplicatedPhones.txt
            using var dupPhonesStream = zipArchive.Entries.First(entry => entry.FullName == "DuplicatedPhones.txt").Open();
            using var dupPhonesReader = new StreamReader(dupPhonesStream);
            var dupPhonesContent = await dupPhonesReader.ReadToEndAsync();
            Assert.Contains("11988888888", dupPhonesContent);
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
            var exception = await Assert.ThrowsAsync<Exception>(() => zipService.CreateZipFileAsync(files, "report", new List<string>(), new List<RemovedRowDetail>()));
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