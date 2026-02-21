using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using CampaignBaseAPI.Constants;
using CampaignBaseAPI.Services.Interface;

namespace CampaignBaseAPI.Services
{
    public class ZipService : IZipService
    {
        public async Task<byte[]> CreateZipFileAsync(Dictionary<string, MemoryStream> files)
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