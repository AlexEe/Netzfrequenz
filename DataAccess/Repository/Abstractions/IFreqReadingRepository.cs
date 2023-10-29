using DataAccess.Entities;

namespace DataAccess.Repository
{
    public interface IFreqReadingRepository
    {
        Task<IEnumerable<FreqReading>> GetReadings(int limit);
        Task<FreqReading> GetLatestReading();
        Task<IEnumerable<FreqReading>> GetReadingsForPeriod(DateTimeOffset start, DateTimeOffset end);
        Task AddAndSave(IEnumerable<FreqReading> reading, CancellationToken token=default);
    }
}