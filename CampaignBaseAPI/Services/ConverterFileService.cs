using System.Data;
using System.Text;
using CampaignBaseAPI.Constants;
using CampaignBaseAPI.Services.Interface;
using ExcelDataReader;

namespace CampaignBaseAPI.Services
{
    public class ConverterFileService : IConverterFileService
    {
        public async Task<MemoryStream> ConvertFileAsync(IFormFile file)
        {
            if (file is null)
                throw new ArgumentException("File is null or empty.");
            
            await ValidateFileHasDataAsync(file);

            try
            {
                if (file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                {
                    var memoryStream = new MemoryStream(); // cria a variazvel para o tempo de execução do método, depois é descartada
                    await file.CopyToAsync(memoryStream); // copia o arquivo csv para o memoryStream
                    memoryStream.Position = 0; // reseta a posição do stream para o início, para que possa ser lido posteriormente
                    return memoryStream ?? throw new InvalidOperationException("Failed to convert CSV file to MemoryStream.");
                }
                if (file.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase) || file.FileName.EndsWith(".xls", StringComparison.OrdinalIgnoreCase))
                {
                    //stringbuilder é usado para construir o conteúdo do arquivo csv de forma eficiente, evitando a criação de múltiplas strings imutáveis durante a concatenação
                    StringBuilder csvContent = new StringBuilder();
                    using (var stream = new MemoryStream())
                    {
                        // Copy the source data into the memory stream (e.g., from an uploaded file)
                        await file.CopyToAsync(stream);
                        // Reset the stream position to the beginning
                        stream.Position = 0;

                        // Create the reader
                        using (var reader = ExcelReaderFactory.CreateReader(stream))
                        {
                            var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                            {
                                ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                                {
                                    UseHeaderRow = true  // pega o cabeçalho da planilha
                                }
                            });

                            var dataTable = result.Tables[0]; // Pega a primeira tabela (planilha)

                            //verifica o cabeçalho do XLS
                            if(dataTable.Columns.Count == 0 || dataTable.Rows.Count == 0)
                                throw new ArgumentException("Excel file is empty or does not contain data.");

                            //escrever o cabecalho
                            for (int i = 0; i < dataTable.Columns.Count; i++)
                            {
                                if (i > 0) csvContent.Append(","); // Adiciona vírgula entre os nomes das colunas, exceto antes do primeiro
                                csvContent.Append(EscapeField(dataTable.Columns[i].ColumnName)); //tratativa de scape ""
                            }
                            csvContent.AppendLine(); // Nova linha após o cabeçalho

                            //escrever as linhas apartir do cabeçalho
                            foreach (DataRow row in dataTable.Rows)
                            {
                                for (int i = 0; i < dataTable.Columns.Count; i++)
                                {
                                    if (i > 0) csvContent.Append(","); // Adiciona vírgula entre os valores das colunas, exceto antes do primeiro

                                    object cellValue = row[i];
                                    string fieldValue = EscapeField(cellValue?.ToString() ?? ""); //tratativa de scape ""
                                    csvContent.Append(fieldValue); // Adiciona o valor da célula ao conteúdo do CSV
                                }
                                csvContent.AppendLine(); // Nova linha após cada registro
                            }
                        }
                    }
                    return new MemoryStream(Encoding.UTF8.GetBytes(csvContent.ToString()));
                }
                else
                {
                    throw new ArgumentException(string.Format(ReturnMessages.InvalidFileExtension, string.Join(", ", FileConstants.AllowedExtensions)));
                }
            }
            catch (InvalidOperationException)
            {
                throw; // Rethrow the exception to be handled by the caller
            }
            catch (Exception ex)
            {
                // Log the exception (you can use a logging framework like Serilog, NLog, etc.)
                Console.Error.WriteLine($"Error converting file {file?.FileName ?? ""}: {ex.Message}");
                throw; // Rethrow the exception to be handled by the caller
            }
        }

        private static string EscapeField(string field)
        {
            if (string.IsNullOrEmpty(field)) return "";

            if (field.Contains(",") || field.Contains("\"") || field.Contains("\n"))
            {
                // Escape double quotes by doubling them
                string escapedField = field.Replace("\"", "\"\"");
                // Enclose the field in double quotes
                return $"\"{escapedField}\"";
            }
            return field;

        }

        private static async Task<IFormFile>ValidateFileHasDataAsync(IFormFile file)
        {
            using var reader = new StreamReader(file.OpenReadStream());
            var header = await reader.ReadLineAsync() ?? ""; // Lê a primeira linha do arquivo (cabeçalho)
            var dataLine = await reader.ReadLineAsync(); // Lê a segunda linha do arquivo

            if(header.Length == 0 || dataLine is null || dataLine.Length == 0)
                throw new ArgumentException("File is empty or does not contain data.");
                return file;
        }
    }

}