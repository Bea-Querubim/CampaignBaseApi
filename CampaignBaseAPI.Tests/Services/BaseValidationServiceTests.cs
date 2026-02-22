using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using CampaignBaseAPI.Services;
using CampaignBaseAPI.Models;

namespace CampaignBaseAPI.Tests.Services
{
    public class BaseValidationServiceTests
    {
        [Fact]
        public async Task ValidateAndCleanBaseAsync_ShouldProcessCsvCorrectly()
        {
            // Arrange
            var csv = "Phone,Name\n559999999999,Joao\n559999999999,Maria\n99999999999,Ana\n1234567890,Pedro\n,Lucas\n";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));
            var service = new BaseValidationService();

            // Act
            var result = await service.ValidateAndCleanBaseAsync(stream);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(5, result.TotalInputRows);
            Assert.Equal(1, result.DuplicatedRowsRemovedCount);
            Assert.Equal(1, result.EmptyPhoneRowsCount);
            Assert.Equal(1, result.InvalidPhoneRowsCount);
            Assert.Equal(2, result.TotalValidRows);
            Assert.NotNull(result.CleanedFile);
            Assert.Contains("99999999999", Encoding.UTF8.GetString(result.CleanedFile.ToArray()));
            Assert.Contains("Joao", Encoding.UTF8.GetString(result.CleanedFile.ToArray()));
        }
    }
}
