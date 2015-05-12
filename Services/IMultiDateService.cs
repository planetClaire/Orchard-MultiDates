using System;
using Orchard;

namespace MultiDates.Services
{
    public interface IMultiDateService : IDependency {
        DateTime[] ParseMultiDates(string datesString);
        Tuple<DateTime, DateTime> ParseDateRange(string datesString);
    }
}