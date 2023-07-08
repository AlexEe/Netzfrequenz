using API.Entities;
using API.Interfaces;

namespace API.Data
{
    public class FrequencyRepository : iFrequencyRepository
    {
        private readonly DataContext _context;
        public FrequencyRepository(DataContext context)
        {
            _context = context;
            
        }
        public FreqReading Update()
        {
            // Make request to get latest frequency value.
            HttpClient http = new HttpClient();
            var newFreqValue = http.GetAsync("https://www.netzfrequenz.info/json/act.json").Result.Content.ReadAsStringAsync().Result;

            // Store result in db.
            FreqReading newReading = new FreqReading{ Timestamp = DateTime.Now, Frequency = float.Parse(newFreqValue)};
            _context.Add(newReading);
            _context.SaveChanges();
            return newReading;
        }

        Task<FreqReading> iFrequencyRepository.GetLatestReadingAsync()
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<FreqReading>> iFrequencyRepository.GetReadingsAsync()
        {
            throw new NotImplementedException();
        }
    }
}