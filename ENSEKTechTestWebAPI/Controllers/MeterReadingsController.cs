using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ENSEKTechTestWebAPI.Factories;
using ENSEKTechTestWebAPI.Models;

namespace ENSEKTechTestWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MeterReadingsController : ControllerBase
    {
        private readonly ILogger<MeterReadingsController> _logger;

        public MeterReadingsController(ILogger<MeterReadingsController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("GetAccountsList")]
        public IEnumerable<AccountDetails> GetAccountsList()
        {
            var factory = new MeterReadingsFactory();
            var accounts = factory.GetAccounts();
            return accounts;
        }

        [HttpPost, DisableRequestSizeLimit]
        [Route("MeterReadingUploads")]
        public List<UploadResults> MeterReadingUploads(List<MeterReading> meterReadings)
        {
            var factory = new MeterReadingsFactory();
            var results = factory.SaveMeterReadings(meterReadings);
            return results;
        }

    }
}
