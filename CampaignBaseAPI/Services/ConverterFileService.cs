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
                    var memoryStream = new MemoryStream();
                    await file.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;
                    return memoryStream ?? throw new InvalidOperationException("Failed to convert CSV file to MemoryStream.");
                }
                if (file.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase) || file.FileName.EndsWith(".xls", StringComparison.OrdinalIgnoreCase))
                {
                    var csvContent = new StringBuilder();
                    using (var stream = new MemoryStream())
                    {
                        await file.CopyToAsync(stream);
                        stream.Position = 0;

                        using (var reader = ExcelReaderFactory.CreateReader(stream))
                        {
                            var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                            {
                                ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                                {
                                    UseHeaderRow = true
                                }
                            });

                            var dataTable = result.Tables[0];

                            if (dataTable.Columns.Count == 0 || dataTable.Rows.Count == 0)
                                throw new ArgumentException("Excel file is empty or does not contain data.");

                            for (int i = 0; i < dataTable.Columns.Count; i++)
                            {
                                if (i > 0) csvContent.Append(",");
                                csvContent.Append(EscapeField(dataTable.Columns[i].ColumnName));
                            }
                            csvContent.AppendLine();

                            foreach (DataRow row in dataTable.Rows)
                            {
                                for (int i = 0; i < dataTable.Columns.Count; i++)
                                {
                                    if (i > 0) csvContent.Append(",");

                                    object cellValue = row[i];
                                    string fieldValue = EscapeField(cellValue?.ToString() ?? "");
                                    csvContent.Append(fieldValue);
                                }
                                csvContent.AppendLine();
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
                throw;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error converting file {file?.FileName ?? ""}: {ex.Message}");
                throw;
            }
        }

        private static string EscapeField(string field)
        {
            if (string.IsNullOrEmpty(field)) return "";

            if (field.Contains(",") || field.Contains("\"") || field.Contains("\n"))
            {
                string escapedField = field.Replace("\"", "\"\"");
                return $"\"{escapedField}\"";
            }
            return field;

        }

        private static async Task<IFormFile> ValidateFileHasDataAsync(IFormFile file)
        {
            using var reader = new StreamReader(file.OpenReadStream());
            var header = await reader.ReadLineAsync() ?? "";
            var dataLine = await reader.ReadLineAsync();

            if (header.Length == 0 || dataLine is null || dataLine.Length == 0)
                throw new ArgumentException("File is empty or does not contain data.");
            return file;
        }
    }

}