using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using CampaignBaseAPI.Services;
using CampaignBaseAPI.Models;
using static CampaignBaseAPI.Constants.ReturnMessages;

namespace CampaignBaseAPI.Tests.Services
{
    public class BaseValidationServiceTests
    {
        [Fact]
        public async Task ValidateAndCleanBaseAsync_ShouldProcessCsvCorrectly()
        {
            // Cenário: CSV com telefone válido, duplicado, inválido e vazio
            var csv = "Phone,Name\n559999999999,Joao\n559999999999,Maria\n11988888888,Ana\n1187777777,Pedro\n,Lucas\n12345,Invalido\n";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));
            var service = new BaseValidationService();

            // Act
            var result = await service.ValidateAndCleanBaseAsync(stream);

            // Assert
            Assert.NotNull(result);

            // Total de linhas processadas (inclui todas as linhas de dados)
            Assert.Equal(10, result.TotalInputRows);
            // Duplicados: nenhum nesse cenário com a regra atual
            Assert.Equal(0, result.DuplicatedRowsRemovedCount);
            // Telefones vazios: Lucas
            Assert.Equal(1, result.EmptyPhoneRowsCount);
            // Telefones inválidos: Joao, Maria, Pedro e Invalido
            Assert.Equal(4, result.InvalidPhoneRowsCount);
            // O arquivo limpo deve conter apenas os válidos
            Assert.NotNull(result.CleanedFile);
            var cleanedCsv = Encoding.UTF8.GetString(result.CleanedFile.ToArray());
            // O número "12345" deve ser considerado inválido
            Assert.DoesNotContain("12345,Invalido", cleanedCsv);

            // Telefones válidos: apenas Ana
            Assert.Equal(1, result.TotalValidRows);

            Assert.Contains("11988888888,Ana", cleanedCsv); // válido
            Assert.DoesNotContain("559999999999,Joao", cleanedCsv); // inválido pela regra atual
            Assert.DoesNotContain(",Lucas", cleanedCsv); // vazio
            Assert.DoesNotContain("559999999999,Maria", cleanedCsv); // duplicado
        }

        [Fact]
        public async Task ValidateAndCleanBaseAsync_ShouldThrowArgumentException_WhenNoValidRowsRemain()
        {
            // Cenário: todas as linhas são inválidas/vazias/duplicadas após normalização
            var csv = "Phone,Name\n12345,Invalido1\n,SemTelefone\n12345,Invalido2\n";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));
            var service = new BaseValidationService();

            // Act
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => service.ValidateAndCleanBaseAsync(stream));

            // Assert
            Assert.Equal(NoValidRowsAfterValidation, exception.Message);
        }
    }
}
