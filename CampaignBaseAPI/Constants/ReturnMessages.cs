using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CampaignBaseAPI.Constants
{
    public static class ReturnMessages
    {
        public const string EmptyFile = "Empty file uploaded.";
        public const string SheetProcessedSuccessfully = "Sheets {0} processed successfully.";
        public const string SizedRequired = "Size is required, minimum value is 1000.";
        public const string FileRequired = "File is required.";
        public const string InvalidFileExtension = "Invalid file extension. Use allowed extensions: {0}.";
    }
}