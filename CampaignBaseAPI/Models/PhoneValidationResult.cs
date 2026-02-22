using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static CampaignBaseAPI.Enums.RemovalReason;

namespace CampaignBaseAPI.Models
{
    public class PhoneValidationResult
    {
        public bool IsValid { get; set; } = false;
        public string OriginalPhone { get; set; } = string.Empty;
        public string NormalizedNumber { get; set; } = string.Empty;
        public Reasons? Reason { get; set; }
    }
}