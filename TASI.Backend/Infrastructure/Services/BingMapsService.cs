using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using TASI.Backend.Infrastructure.Configs;
using TASI.Backend.Infrastructure.Helpers;

namespace TASI.Backend.Infrastructure.Services
{
    public interface IBingMapsService
    {
        Task<double> CalculateDistance(double sourceLatitude, double sourceLongitude, double destinationLatitude,
            double destinationLongitude, CancellationToken cancellationToken);

        Task<ReverseGeocodedAddress> ReverseGeocode(string address, CancellationToken cancellationToken);

        bool IsPointDifferent(double latitude1, double longitude1, double latitude2, double longitude2,
            double tolerance = 0.001);
    }

    public class BingMapsService : IBingMapsService
    {
        private readonly CultureInfo USCulture = new("en-US");

        private readonly BingMapsConfig _config;
        private readonly IHttpClientFactory _httpClientFactory;

        public BingMapsService(IOptions<BingMapsConfig> bingConfig, IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _config = bingConfig.Value;
        }

        public async Task<double> CalculateDistance(double sourceLatitude, double sourceLongitude,
            double destinationLatitude, double destinationLongitude, CancellationToken cancellationToken)
        {
            var queries = new Dictionary<string, string>
            {
                {"origins", $"{sourceLatitude.ToString(USCulture)},{sourceLongitude.ToString(USCulture)}"},
                {
                    "destinations",
                    $"{destinationLatitude.ToString(USCulture)},{destinationLongitude.ToString(USCulture)}"
                },
                {"travelMode", "driving"},
                {"timeUnit", "minute"},
                {"key", _config.ApiKey}
            };

            var url = "https://dev.virtualearth.net/REST/v1/Routes/DistanceMatrix" + queries.ToQueryString();
            var client = _httpClientFactory.CreateClient();
            var result = await client.GetAsync(url, cancellationToken);
            result.EnsureSuccessStatusCode();
            var tokenRoot = JToken.Parse(await result.Content.ReadAsStringAsync(cancellationToken));
            return tokenRoot["resourceSets"]?.First()["resources"]?.First()["results"]?.First()["travelDistance"]
                ?.ToObject<double>() ?? -1;
        }

        public async Task<ReverseGeocodedAddress> ReverseGeocode(string address, CancellationToken cancellationToken)
        {
            var queries = new Dictionary<string, string>
            {
                {"q", address},
                {"key", _config.ApiKey}
            };

            var url = "https://dev.virtualearth.net/REST/v1/Locations" + queries.ToQueryString();
            var client = _httpClientFactory.CreateClient();
            var result = await client.GetAsync(url, cancellationToken);
            result.EnsureSuccessStatusCode();
            var tokenRoot = JToken.Parse(await result.Content.ReadAsStringAsync(cancellationToken));
            var entry = tokenRoot["resourceSets"]?.First()["resources"]?.FirstOrDefault();
            if (entry == null)
            {
                return new ReverseGeocodedAddress(false, "", "", 0, 0);
            }

            var coordinates = entry["point"]?["coordinates"]?.ToObject<double[]>();
            Debug.Assert(coordinates != null, nameof(coordinates) + " != null");
            return new ReverseGeocodedAddress(true, address, entry?["name"]?.Value<string>(), coordinates[0],
                coordinates[1]);
        }

        public bool IsPointDifferent(double latitude1, double longitude1, double latitude2, double longitude2,
            double tolerance = 0.001)
        {
            return Math.Abs(latitude1 - latitude2) > tolerance || Math.Abs(longitude1 - longitude2) > tolerance;
        }
    }
}
