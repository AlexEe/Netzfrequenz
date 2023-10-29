using System.Globalization;
using System.IO.Compression;
using System.Net.Cache;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using AutoMapper;
using Contract.DTOs;
using DataAccess.Entities;
using DataAccess.Repository;
using Logic.Services.Abstractions;
using Microsoft.Extensions.Logging;

namespace Logic.Services
{
    internal class FreqReadingService : IFreqReadingService
    {

        private readonly IFreqReadingRepository _repository;
        private readonly IMapper _mapper;
        private ILogger _logger { get; }

        public FreqReadingService(IFreqReadingRepository repository, IMapper mapper, ILogger<FreqReadingService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<FreqReadingDto> Update()
        {
            // Make request to get latest frequency value.
            try {
                HttpResponseMessage response = await CallUrl("https://netzfrequenzmessung.de:9081/frequenz02a.xml?c=2651089");
                response.EnsureSuccessStatusCode();
                var newReading = await ParseResponse(response);
                
                await _repository.AddAndSave(new []{newReading});
                return _mapper.Map<FreqReadingDto>(newReading);
            }
            // Catch exception if request is unsuccessful.
            catch(HttpRequestException e) {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task<IEnumerable<FreqReadingDto>> GetReadings(int limit)
        {
            var readings = await _repository.GetReadings(limit);
            return _mapper.Map<IEnumerable<FreqReadingDto>>(readings);
        }

        public async Task<IEnumerable<FreqReadingDto>> GetReadingsForPeriod(DateTimeOffset start, DateTimeOffset end)
        {
            var readings = await _repository.GetReadingsForPeriod(start, end);
            return _mapper.Map<IEnumerable<FreqReadingDto>>(readings);
        }

        public async Task<FreqReadingDto> GetLatestReading()
        {
            var reading = await _repository.GetLatestReading();
            return _mapper.Map<FreqReadingDto>(reading);
        }

        private async Task<HttpResponseMessage> CallUrl(string fullUrl)
        {
            HttpClient client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, fullUrl);
            var header = new ProductInfoHeaderValue("Netzfrequenz", "0.0.1");
            request.Headers.UserAgent.Add(header);
            var response = await client.SendAsync(request);
            return response;
        }

        public async Task<FreqReading> ParseResponse(HttpResponseMessage response)
        {
            var respString = await response.Content.ReadAsStringAsync();
            var rgFrequency = new Regex(@"(?<=<f2>)(.*?)(?=<)");
            var rgTimestamp = new Regex(@"(?<=<z> )(.*?)(?=<)");
            var frequency = float.Parse(rgFrequency.Matches(respString)[0].Value);
            var timestamp = DateTimeOffset.Parse(rgTimestamp.Matches(respString)[0].Value, CultureInfo.GetCultureInfo("de-DE"), DateTimeStyles.AssumeUniversal);
            var newReading = new FreqReading{ Timestamp = timestamp, Frequency = frequency};
            return newReading;
        }
    }
}
