using System;
using Orchard.Localization.Models;
using Orchard.Localization.Services;

namespace MultiDates.Services
{
    public class MultiDateService : IMultiDateService
    {
        private readonly IDateLocalizationServices _dateLocalizationServices;

        public MultiDateService(IDateLocalizationServices dateLocalizationServices) {
            _dateLocalizationServices = dateLocalizationServices;
        }

        public DateTime[] ParseMultiDates(string datesString) {
            if (!string.IsNullOrEmpty(datesString)) {
                var options = new DateLocalizationOptions {EnableTimeZoneConversion = false};
                var dateStrings = datesString.Split(',');
                var dates = new DateTime[dateStrings.Length];
                for (var i = 0; i < dateStrings.Length; i++) {
                    dates[i] = _dateLocalizationServices.ConvertFromLocalizedString(dateStrings[i], "00:00:00 AM", options).GetValueOrDefault();
                }
                return dates;
            }
            return null;
        }

        public Tuple<DateTime, DateTime> ParseDateRange(string datesString) {
            if (!string.IsNullOrEmpty(datesString)) {
                var options = new DateLocalizationOptions {EnableTimeZoneConversion = false};
                var dateStrings = datesString.Split(new[] {" - "}, StringSplitOptions.RemoveEmptyEntries);
                return new Tuple<DateTime, DateTime>(_dateLocalizationServices.ConvertFromLocalizedString(dateStrings[0], "00:00:00 AM", options).GetValueOrDefault(), _dateLocalizationServices.ConvertFromLocalizedString(dateStrings[1], "00:00:00 AM", options).GetValueOrDefault());
            }
            return null;
        }
    }
}