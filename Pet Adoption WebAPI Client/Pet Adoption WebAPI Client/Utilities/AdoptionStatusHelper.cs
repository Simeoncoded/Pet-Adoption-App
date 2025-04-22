using Pet_Adoption_WebAPI_Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pet_Adoption_WebAPI_Client.Utilities
{
    public static class AdoptionStatusHelper
    {
        public static IEnumerable<AdoptionStatus> AdoptionStatuses { get; } = Enum.GetValues(typeof(AdoptionStatus)).Cast<AdoptionStatus>();
    }
}
