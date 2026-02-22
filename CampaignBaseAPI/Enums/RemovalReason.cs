using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CampaignBaseAPI.Enums
{
    public class RemovalReason
    {
        public enum  Reasons : short
        {
            EmptyPhone = 1,
            InvalidPhone =2,
            DuplicatedPhone =3,
            ErrorNormalizingValidatePhone = 4

        }
    }
}