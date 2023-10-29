using Microsoft.AspNetCore.Mvc;
using DataAccess.Repository;
using DataAccess.Entities;
using Contract.DTOs;
using Logic.Services.Abstractions;
using System.Diagnostics;

namespace API.Controllers
{
    [ApiController]
    [Route("api/frequency")]
    public class FreqReadingController : ControllerBase
    {
        private readonly IFreqReadingService _service;

        public FreqReadingController(IFreqReadingService service)
        {
            _service = service;
        }

        /// <summary>
        /// This request gets limited list of frequency readings from the database.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FreqReadingDto>>> GetReadings(int limit)
        {
            var getReadings = await _service.GetReadings(limit);
            return Ok(getReadings);
        }

        /// <summary>
        /// This request gets a list of frequency readings from the database for a given time period.
        /// </summary>
        [HttpGet("past")]

        public async Task<ActionResult<IEnumerable<FreqReadingDto>>> GetReadingsForPeriod(DateTimeOffset start, DateTimeOffset end)
        {
            var getReadingsForPeriod = await _service.GetReadingsForPeriod(start, end);
            return Ok(getReadingsForPeriod);
        }

        /// <summary>
        /// This request gets the latest frequency reading from the database.
        /// </summary>
        [HttpGet("latest")]
        public async Task<ActionResult<FreqReadingDto>> GetLatestReading()
        {
            return await _service.GetLatestReading();
        }

        /// <summary>
        /// This request gets the current frequency value from netzfrequenz.info and stores it in the database.
        /// </summary>
        [HttpGet("update")]
        public async Task<ActionResult<FreqReadingDto>> Update()
        {
            return await _service.Update();
        }
    }
}
