using ClosedXML.Excel;
using System.Text;
using Microsoft.AspNetCore.Http;
using CampaignBaseAPI.Services;

namespace CampaignBaseAPI.Tests.Services
{
    public class ConverterFileServiceTests
    {
        public ConverterFileServiceTests()
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        }

        [Fact]
        public async Task ConvertFileAsync_ShouldThrowException_WhenFileIsNull()
        {
            //Arrange → preparar cenário
            ConverterFileService converterFileService = new ConverterFileService();
            //Act → executar ação
             IFormFile? file = null;
            //Assert & act → verificar resultado e executar ação
            // Assert.ThrowsAsync é usado para verificar se a chamada do método ConvertFileAsync lança uma exceção do tipo ArgumentException quando o arquivo é nulo. O teste passa se a exceção for lançada, indicando que o método está lidando corretamente com a entrada nula.
            await Assert.ThrowsAsync<ArgumentException>(() => converterFileService.ConvertFileAsync(file!));
        }


        [Fact]
        public async Task ConvertFileAsync_ShouldThrowException_WhenFileHaventData()
        {
            // Arrange
            ConverterFileService converterFileService = new ConverterFileService();
            var fileStreamMock = new MemoryStream(Encoding.UTF8.GetBytes("phone, link, name\n")); // Simula um arquivo CSV vazio (apenas o cabeçalho) nome, link e name como string, sao convertidos para bytes e depois para um MemoryStream
            FormFile file = new FormFile(fileStreamMock, 0, fileStreamMock.Length, "file", "file.csv");
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => converterFileService.ConvertFileAsync(file));
        }

        [Fact]
        public async Task ConvertFileAsync_ShouldReturnMemoryStream_WhenCsvFileIsValid()
        {
            // Arrange
            ConverterFileService converterFileService = new ConverterFileService();
            var fileStreamMock = new MemoryStream(Encoding.UTF8.GetBytes("phone,link,name\n11988888888,https://s3.image.teste.com/teste.jpg, John Doe\n11988888887,https://s3.image.teste.com/teste2.jpg, John Park\n"));

            FormFile file = new FormFile(fileStreamMock, 0, fileStreamMock.Length, "file", "file.csv");

            // Act 
            var response = await converterFileService.ConvertFileAsync(file);

            // Assert
            Assert.NotNull(response);
            Assert.True(response.Length > 0);
        }


        [Fact]
        public async Task ConvertFileAsync_ShouldReturnMemoryStream_WhenExcelFileIsValid()
        {
            //arrange
            ConverterFileService converterFileService = new ConverterFileService();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Sheet1");

            worksheet.Cell(1, 1).Value = "phone";
            worksheet.Cell(1, 2).Value = "link";
            worksheet.Cell(1, 3).Value = "name";

            worksheet.Cell(2, 1).Value = "11988888888";
            worksheet.Cell(2, 2).Value = "https://s3.image.teste.com/teste.jpg";
            worksheet.Cell(2, 3).Value = "John Doe";

            var excelStream = new MemoryStream();
            workbook.SaveAs(excelStream);
            excelStream.Position = 0; // volta para o inicio do arquivo
            FormFile file = new FormFile(excelStream, 0, excelStream.Length, "file", "file.xlsx");

            //act
            var response = await converterFileService.ConvertFileAsync(file);

            //assert
            Assert.NotNull(response);
            Assert.True(response.Length > 0);
        }

        [Theory]
        [InlineData("file.txt")]
        [InlineData("file.docx")]
        [InlineData("file.pdf")]
        public async Task ConvertFileAsync_ShouldThrowException_WhenFileIsNotCsvOrExcel(string fileName)
        {
            // Arrange
            ConverterFileService converterFileService = new ConverterFileService();
            var fileStreamMock = new MemoryStream(Encoding.UTF8.GetBytes("phone,link,name\n11988888888,https://s3.image.teste.com/teste.jpg, John Doe\n11988888887,https://s3.image.teste.com/teste2.jpg, John Park\n"));
            FormFile fileTxt = new FormFile(fileStreamMock, 0, fileStreamMock.Length, "file", fileName);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => converterFileService.ConvertFileAsync(fileTxt));
        }

        [Fact]
        public async Task ConvertFileAsync_ShouldThrowException_WhenExcelFileHasOnlyHeader()
        {
            // Arrange
            ConverterFileService converterFileService = new ConverterFileService();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Sheet1");

            worksheet.Cell(1, 1).Value = "phone";
            worksheet.Cell(1, 2).Value = "link";
            worksheet.Cell(1, 3).Value = "name";

            var excelStream = new MemoryStream();
            workbook.SaveAs(excelStream);
            excelStream.Position = 0; // volta para o inicio do arquivo
            FormFile file = new FormFile(excelStream, 0, excelStream.Length, "file", "file.xlsx");

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => converterFileService.ConvertFileAsync(file));
        }
    }
}
