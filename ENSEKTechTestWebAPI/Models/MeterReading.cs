using System;
using System.Collections.Generic;

#nullable disable

namespace ENSEKTechTestWebAPI.Models
{
    public partial class MeterReading
    {
        public int AccountId { get; set; }
        public DateTime? MeterReadingDateTime { get; set; }
        public int? MeterReadValue { get; set; }
    }
}
