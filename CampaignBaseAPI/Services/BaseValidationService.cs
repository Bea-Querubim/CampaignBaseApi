using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CampaignBaseAPI.Constants.ReturnMessages;
using static CampaignBaseAPI.Enums.RemovalReason.Reasons;
using CampaignBaseAPI.Services.Interface;
using CampaignBaseAPI.Models;

namespace CampaignBaseAPI.Services
{
    public class BaseValidationService : IBaseValidationService
    {
        public async Task<BaseValidationResult> ValidateAndCleanBaseAsync(MemoryStream file)
        {
            try
            {
                var validationBaseResult = new BaseValidationResult();
                var cleanedFile = new StringBuilder();
                using var reader = new StreamReader(file);

                var phoneNumbersDeduplicate = new HashSet<string>();
                var header = await reader.ReadLineAsync(); // Lê o cabeçalho do arquivo CSV
                var line = await reader.ReadLineAsync(); // Lê a primeira linha de dados do arquivo CSV
                var countLines = 2; // inicia em 2 pulando o cabeçalho

                if (line == null || string.IsNullOrWhiteSpace(line))
                    throw new ArgumentException(EmptyFile);

                do
                {

                    var phoneValidationResult = NormalizeAndValidatePhone(line);

                    if (!phoneValidationResult.IsValid)
                    {
                        var removedRowDetail = new RemovedRowDetail
                        {
                            OriginalPhone = phoneValidationResult.OriginalPhone,
                            NormalizedPhone = phoneValidationResult.NormalizedNumber,
                            Reason = phoneValidationResult.Reason,
                            RowNumber = countLines
                        };
                        validationBaseResult.RemovedRowDetails!.Add(removedRowDetail);

                        switch (phoneValidationResult.Reason)
                        {
                            case EmptyPhone:
                                validationBaseResult.EmptyPhoneRowsCount++;
                                break;

                            case InvalidPhone:
                                validationBaseResult.InvalidPhoneRowsCount++;
                                break;
                            default:
                                validationBaseResult.ErrorsValidationRowsCount++;
                                break;
                        }
                        validationBaseResult.TotalInputRows++;
                        countLines++;
                    }
                    else if (!phoneNumbersDeduplicate.Add(phoneValidationResult.NormalizedNumber)) // Verifica se o número já existe no HashSet, retorna true se o número foi adicionado, false se já existe
                    {
                        phoneValidationResult.IsValid = false;
                        phoneValidationResult.Reason = DuplicatedPhone;

                        var removedRowDetail = new RemovedRowDetail
                        {
                            OriginalPhone = phoneValidationResult.OriginalPhone,
                            NormalizedPhone = phoneValidationResult.NormalizedNumber,
                            Reason = phoneValidationResult.Reason,
                            RowNumber = countLines
                        };

                        validationBaseResult.RemovedRowDetails!.Add(removedRowDetail);

                        validationBaseResult.DuplicatedRowsRemovedCount++;
                        validationBaseResult.DuplicatedPhonesNormalized!.Add(phoneValidationResult.NormalizedNumber);
                    }
                    else
                    {
                        var fields = line.Split(",");
                        fields[0] = phoneValidationResult.NormalizedNumber;
                        cleanedFile.AppendLine(string.Join(",", fields)); validationBaseResult.TotalValidRows++;
                    }

                    validationBaseResult.TotalInputRows++;
                    countLines++;
                } while ((line = await reader.ReadLineAsync()) != null);

                if (cleanedFile.Length > 0)
                {
                    validationBaseResult.CleanedFile = new MemoryStream(Encoding.UTF8.GetBytes(header + Environment.NewLine + cleanedFile.ToString()));
                }
                return validationBaseResult;

            }
            catch (Exception ex)
            {
                return ex is ArgumentException ? throw new ArgumentException(ex.Message) : throw new Exception(ErrorValidatingFields);
            }
        }

        private PhoneValidationResult NormalizeAndValidatePhone(string phoneNumber)
        {
            var validationReturn = new PhoneValidationResult();
            try
            {
                var digits = new string(phoneNumber.Where(char.IsDigit).ToArray()); // remove + () espaços - e .

                if (digits.StartsWith("55"))
                    digits = digits.Substring(2); // remove o DDI 55

                if (string.IsNullOrEmpty(digits))
                {
                    validationReturn.OriginalPhone = phoneNumber;
                    validationReturn.NormalizedNumber = digits;
                    validationReturn.Reason = EmptyPhone;

                    return validationReturn;
                }


                if (!(digits.Length == 11) || (digits.Length == 10))
                {
                    validationReturn.OriginalPhone = phoneNumber;
                    validationReturn.NormalizedNumber = digits;
                    validationReturn.Reason = InvalidPhone;

                    return validationReturn;
                }

                validationReturn.IsValid = true;
                validationReturn.NormalizedNumber = digits;
                validationReturn.OriginalPhone = phoneNumber;
                validationReturn.Reason = null;
                return validationReturn;

            }
            catch
            {
                return new PhoneValidationResult{
                    IsValid = false,
                    NormalizedNumber = string.Empty,
                    OriginalPhone = phoneNumber,
                    Reason = ErrorNormalizingValidatePhone
                };
            }
        }
    }
}