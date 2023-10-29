using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contract.DTOs;

namespace Logic.Services.Abstractions
{
    public interface IFreqReadingService
    {
        Task<FreqReadingDto> Update();
        Task<IEnumerable<FreqReadingDto>> GetReadings(int limit);
        Task <IEnumerable<FreqReadingDto>> GetReadingsForPeriod(DateTimeOffset start, DateTimeOffset end);
        Task<FreqReadingDto> GetLatestReading();
    }
}
