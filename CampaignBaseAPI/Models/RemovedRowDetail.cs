using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static CampaignBaseAPI.Enums.RemovalReason;

namespace CampaignBaseAPI.Models
{
    public class RemovedRowDetail
    {
        public int RowNumber { get; set; }  = 2;
        public string OriginalPhone { get; set; } = string.Empty;
        public string NormalizedPhone { get; set; } = string.Empty;
        public Reasons? Reason { get; set; }
    }
}