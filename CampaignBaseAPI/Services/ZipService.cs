using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using CampaignBaseAPI.Constants;
using CampaignBaseAPI.Models;
using CampaignBaseAPI.Services.Interface;

namespace CampaignBaseAPI.Services
{
    public class ZipService : IZipService
    {
        public async Task<byte[]> CreateZipFileAsync(Dictionary<string, MemoryStream> files, string report, List<string> duplicatedPhones, List<RemovedRowDetail> removedRowDetails)
        {
            ArgumentNullException.ThrowIfNull(files);

            if (files.Count == 0)
                throw new ArgumentException(ReturnMessages.EmptyFileZipException, nameof(files));
            try
            {
                using var stream = new MemoryStream(); //fecha depois de retornar o array de bytes

                using (var zipArchive = new ZipArchive(stream, ZipArchiveMode.Create, leaveOpen: true)) //fecha antes de retornar o array de bytes
                {
                    foreach (var file in files)
                    {
                        file.Value.Position = 0;
                        var entry = zipArchive.CreateEntry(file.Key, CompressionLevel.Fastest);
                        using var entryStream = entry.Open();
                        await file.Value.CopyToAsync(entryStream);
                    }
                
                    // Add report, duplicated phones, and removed row details as separate entries in the zip
                    var reportEntry = zipArchive.CreateEntry("Report.txt", CompressionLevel.Fastest);
                    using (var reportStream = reportEntry.Open())
                    using (var writer = new StreamWriter(reportStream))
                    {
                        await writer.WriteAsync(report);
                        await writer.WriteLineAsync("\n-------* Details of Removed Row *-------\n");
                        await writer.WriteLineAsync(string.Join(Environment.NewLine, removedRowDetails.Select(d => $"Row {d.RowNumber}: {d.OriginalPhone} -> {d.NormalizedPhone}, Reason: {d.Reason}")));
                    }
                    
                    if(duplicatedPhones != null && duplicatedPhones.Count > 0)
                    {
                        var duplicatedPhonesEntry = zipArchive.CreateEntry("DuplicatedPhones.txt", CompressionLevel.Fastest);
                        using (var duplicatedPhonesStream = duplicatedPhonesEntry.Open())
                        using (var writer = new StreamWriter(duplicatedPhonesStream))
                        {
                            foreach (var phone in duplicatedPhones)
                            {
                                await writer.WriteLineAsync(phone);
                            }
                        }
                    }
                }
                return stream.ToArray();
            }
            catch (Exception e)
            {
                throw new Exception(ReturnMessages.ZipErrorMontage, e);
            }
        }
    }
}