using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ENSEKTechTestWebAPI.Models
{
    public class UploadResults
    {
        public int AccountId { get; set; }
        public DateTime? MeterReadingDateTime { get; set; }
        public int? MeterReadValue { get; set; }

        public string Result { get; set; }
    }
}
