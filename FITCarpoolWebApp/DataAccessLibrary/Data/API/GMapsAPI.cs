﻿using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Json;
using System.Text.Json;
namespace DataAccessLibrary.Data.API
{
    public class GMapsAPI : IGMapsAPI
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;

        public GMapsAPI(IConfiguration config, HttpClient httpClient)
        {
            _config = config;
            _httpClient = httpClient;
        }

        public string MapAPI => _config.GetConnectionString("MapAPI");

        public async Task<string> GetLocationInfoAsync(double latitude, double longitude)
        {
            var apiKey = MapAPI;
            var requestUri = $"https://maps.googleapis.com/maps/api/geocode/json?latlng={latitude},{longitude}&key={apiKey}";
            var response = await _httpClient.GetAsync(requestUri);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var geocodeResponse = JsonSerializer.Deserialize<GeocodeResponse>(json); // Use JsonSerializer here

                // Extract city and state from the response
                var city = "";
                var state = "";
                foreach (var component in geocodeResponse.results[0].address_components)
                {
                    if (component.types.Contains("locality"))
                    {
                        city = component.long_name;
                    }
                    else if (component.types.Contains("administrative_area_level_1"))
                    {
                        state = component.short_name; // Use short_name for state code
                    }
                }

                return $"{city}, {state}";
            }
            return "Location not found";
        }

        private class GeocodeResponse
        {
            public GeocodeResult[] results { get; set; }
        }

        private class GeocodeResult
        {
            public AddressComponent[] address_components { get; set; }
        }

        private class AddressComponent
        {
            public string long_name { get; set; }
            public string short_name { get; set; }
            public string[] types { get; set; }
        }
    }
}
