using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CampaignBaseAPI.Models
{
    public class BaseValidationResult
    {
        public MemoryStream? CleanedFile { get; set; }
        public int TotalInputRows { get; set; } = 0; 
        public int TotalValidRows { get; set; } = 0;
        public int ErrorsValidationRowsCount { get; set; } = 0;
        public int EmptyPhoneRowsCount { get; set; } = 0;
        public int InvalidPhoneRowsCount { get; set; } = 0;
        public int DuplicatedRowsRemovedCount { get; set; } = 0;
        public List<string>? DuplicatedPhonesNormalized { get; set; } = new List<string>();
        public List<RemovedRowDetail>? RemovedRowDetails { get; set; } = new List<RemovedRowDetail>();
    }
}