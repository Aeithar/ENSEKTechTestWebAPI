using System;
using System.Collections.Generic;

#nullable disable

namespace ENSEKTechTestWebAPI.Models
{
    public partial class AccountDetails
    {
        public int AccountId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? MostRecentReading { get; set; }
        public int? CurrentRead { get; set; }
    }
}
