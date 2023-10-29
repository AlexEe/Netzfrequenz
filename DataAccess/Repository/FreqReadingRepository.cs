using System;
using API.Data;
using DataAccess.Entities;
using DataAccess.DB;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repository
{
    public class FreqReadingRepository : IFreqReadingRepository
    {
        private readonly DataContext _context;
        public FreqReadingRepository(DataContext context)
        {
            _context = context;
            
        }

        public async Task<IEnumerable<FreqReading>> GetReadings(int limit)
        {
            var minuteAgo = DateTime.Now.AddMinutes(-2);
            var readings = await _context.Readings
                .OrderByDescending(x => x.Timestamp)
                .Take(limit)
                .Where(x => x.Timestamp >= minuteAgo)
                .ToListAsync();
            return readings;
        }

        public async Task<IEnumerable<FreqReading>> GetReadingsForPeriod(DateTimeOffset start, DateTimeOffset end)
        {
            var readings = await _context.Readings
                .OrderByDescending(x => x.Timestamp)
                .Where(x => x.Timestamp <= end) 
                .Where(x => x.Timestamp >= start) 
                .ToListAsync();
            return readings;
        }

        public async Task<FreqReading> GetLatestReading()
        {
            var latestId = await _context.Readings.MaxAsync(x => x.Id);
            return await _context.Readings.FirstOrDefaultAsync(x => x.Id == latestId);
        }

        public async Task AddAndSave(IEnumerable<FreqReading> reading, CancellationToken token=default)
        {
            await _context.AddRangeAsync(reading, token);
            await _context.SaveChangesAsync(token);
        }
    }
}