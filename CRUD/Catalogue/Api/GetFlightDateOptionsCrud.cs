using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System.Globalization;
using to_integrations.CRUD.Catalogue.Api;

namespace to_integrations.CRUD.Catalogue
{
    public class GetFlightDateOptionsCrud
    {
        private readonly GetFlightDateOptionsApi _api = new();

        private JObject? _response;
        private JArray? _availableDates;

        public int LastStatus { get; private set; }
        private string _lastUrl = string.Empty;

        public async Task CallAsync(
            string cityUid,
            string countryCode,
            bool isFrom,
            int month,
            int year,
            string token)
        {
            _lastUrl =
                $"/api/Catalogue/GetFlightDateOptions" +
                $"?CityUID={cityUid}&CountryCode={countryCode}" +
                $"&IsFrom={isFrom}&Month={month}&Year={year}";

            var (status, body) = await _api.CallAsync(
                cityUid,
                countryCode,
                isFrom,
                month,
                year,
                token);

            LastStatus = status;

            try
            {
                _response = JObject.Parse(body);
                _availableDates = _response["availableDates"] as JArray;
            }
            catch
            {
                _response = null;
                _availableDates = null;
            }
        }

        /* =============================
           VALIDATIONS
           ============================= */

        public bool HasAvailableDatesArray()
            => _availableDates != null;

        public bool AvailableDatesEmpty()
            => _availableDates != null && !_availableDates.Any();

        public bool EachDateHasFields(IEnumerable<string> fields)
        {
            if (_availableDates == null)
                return false;

            foreach (var date in _availableDates)
            {
                foreach (var field in fields)
                {
                    if (date?[field] == null)
                        return false;
                }
            }

            return true;
        }

        public bool HasRemainingTicketLimit()
            => _response?["remainingTicketLimit"] != null;

        public bool RemainingTicketLimitPositive()
        {
            var raw = _response?["remainingTicketLimit"]?.ToString();
            return int.TryParse(raw, out var v) && v > 0;
        }

        public bool DatesWithinMonth(int expectedMonth, int expectedYear)
        {
            if (_availableDates == null)
                return false;

            foreach (var d in _availableDates)
            {
                var raw = d?["departureDate"]?.ToString();

                if (!DateTime.TryParse(raw, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
                    return false;

                if (dt.Month != expectedMonth || dt.Year != expectedYear)
                    return false;
            }

            return true;
        }
    }
}
