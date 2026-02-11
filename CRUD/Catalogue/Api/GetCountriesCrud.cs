using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using to_integrations.HelperMethods;

namespace to_integrations.CRUD.Catalogue.Api
{
    public class GetCountriesCrud
    {
        private readonly GetCountriesApi _api;
        private int _lastStatus;
        private string _lastBody;

        public int LastStatus => _lastStatus;
        public string LastBody => _lastBody;

        public GetCountriesCrud()
        {
            _api = new GetCountriesApi();
        }

        public async Task CallGetCountriesApi(string token = "")
        {
            var effectiveToken = string.IsNullOrEmpty(token) ? TokenCache.CachedToken : token;
            var sw = System.Diagnostics.Stopwatch.StartNew();
            var result = await _api.GetCountriesAsync(effectiveToken);
            sw.Stop();
            LastResponse.TimeMs = sw.ElapsedMilliseconds;
            _lastStatus = result.Status;
            _lastBody = result.Body;
        }

        public bool HasCountriesArray()
        {
            try
            {
                var json = JObject.Parse(_lastBody);
                var countries = json["countries"];
                return countries != null && countries.Type == JTokenType.Array;
            }
            catch
            {
                return false;
            }
        }

        public bool HasErrorFalse()
        {
            try
            {
                var json = JObject.Parse(_lastBody);
                var error = json["error"];
                return error != null && error.Type == JTokenType.Boolean && error.Value<bool>() == false;
            }
            catch
            {
                return false;
            }
        }

        public bool HasCountryCode(string code)
        {
            try
            {
                var json = JObject.Parse(_lastBody);
                var countries = json["countries"] as JArray;
                if (countries == null) return false;
                return countries.Any(c => c["countryCode"]?.ToString() == code);
            }
            catch
            {
                return false;
            }
        }

        public bool ArePhoneCodesValid()
        {
            try
            {
                var json = JObject.Parse(_lastBody);
                var countries = json["countries"] as JArray;
                if (countries == null || !countries.Any()) return false;

                foreach (var country in countries)
                {
                    var phoneCode = country["phoneCode"]?.ToString();
                    if (string.IsNullOrEmpty(phoneCode)) return false;

                    // Valid international dialing code: optional + followed by 1-4 digits
                    if (!Regex.IsMatch(phoneCode.Trim(), @"^\+?\d{1,4}$"))
                        return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ArePhoneCodesNumericString()
        {
            try
            {
                var json = JObject.Parse(_lastBody);
                var countries = json["countries"] as JArray;
                if (countries == null || !countries.Any()) return false;

                foreach (var country in countries)
                {
                    var phoneCode = country["phoneCode"]?.ToString();
                    if (string.IsNullOrEmpty(phoneCode)) return false;

                    // Strip optional leading '+' then check remaining chars are all digits
                    var digits = phoneCode.Trim();
                    if (digits.StartsWith("+"))
                        digits = digits.Substring(1);

                    if (string.IsNullOrEmpty(digits)) return false;
                    if (!digits.All(char.IsDigit)) return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
