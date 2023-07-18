using System.IO.Compression;
using System.Net.Cache;
using System.Net.Http.Headers;
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
            string respString = await response.Content.ReadAsStringAsync();
            string[] separators = {"<f2>", "</f2>", "<z> ", "</z>"};
            string[] results = respString.Split(separators, System.StringSplitOptions.RemoveEmptyEntries);
            var frequency = float.Parse(results[0]);
            DateTime timestamp = DateTime.Parse(results[1]);
            FreqReading newReading = new FreqReading{ Timestamp = timestamp, Frequency = frequency};
            return newReading;
        }
    }
}