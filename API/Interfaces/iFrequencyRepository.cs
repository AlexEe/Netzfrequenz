using Microsoft.AspNetCore.Mvc;
using API.Entities;

namespace API.Interfaces
{
    public interface iFrequencyRepository
    {
        FreqReading Update();

        Task<IEnumerable<FreqReading>> GetReadingsAsync();
        Task<FreqReading> GetLatestReadingAsync();
    }
}