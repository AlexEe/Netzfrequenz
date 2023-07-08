using API.Data;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FrequencyController : ControllerBase
    {
        private readonly iFrequencyRepository _frequencyRepository;

        public FrequencyController(iFrequencyRepository frequencyRepository)
        {
            _frequencyRepository = frequencyRepository;
        }

        /// <summary>
        /// This request gets all frequency readings from the database.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FreqReading>>> GetReadings()
        {
            return Ok(await _frequencyRepository.GetReadingsAsync());
        }

        /// <summary>
        /// This request gets the latest frequency reading from the database.
        /// </summary>
        [HttpGet("latest")]
        public async Task<ActionResult<FreqReading>> GetLatestReading()
        {
            return await _frequencyRepository.GetLatestReadingAsync();
        }

        /// <summary>
        /// This request gets the current frequency value from netzfrequenz.info and stores it in the database.
        /// </summary>
        [HttpGet("update")]
        public FreqReading Update()
        {
            return _frequencyRepository.Update();
        }
    }
}
