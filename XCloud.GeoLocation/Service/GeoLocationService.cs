using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Text;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using XCloud.GeoLocation.Modal;


namespace XCloud.GeoLocation.Service
{
    public class GeoLocationService
    {
        
        public GeoLocationService(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public LocationModal GetLocationByIPAddress(string IPAddress)
        {
            try
            {
                string result = "";
                string IPdippingurl = Configuration.GetSection("IP_dipping_url").Value;
                IPdippingurl = IPdippingurl.Replace("[IPAddr]", IPAddress);
                LocationModal location = new LocationModal();
                WebClient webClient = new WebClient();
                webClient.Encoding = Encoding.UTF8;
                result = webClient.DownloadString(IPdippingurl);
                if (String.IsNullOrEmpty(result) == false)
                {
                    var jsonResult = (JObject)JsonConvert.DeserializeObject(result);
                    string latitude = jsonResult["lat"].Value<string>();
                    string longitude = jsonResult["lon"].Value<string>();

                    location = GetLocationByLatLong(latitude, longitude);
                    location.Latitude = jsonResult["lat"].Value<string>();
                    location.Longitude = jsonResult["lon"].Value<string>();
                    location.CountryCode = jsonResult["countryCode"].Value<string>();
                }

                return location;
            }
            catch (Exception ex)
            {
                //_logger.Error(ex, "Location Error");
                LocationModal location = new LocationModal();
                return location;
            }
        }

        public LocationModal GetLocationByLatLong(string latitude, string longitude)
        {
            try
            {
                string result = "";
                LocationModal locationInfo = new LocationModal();
                WebClient webClient = new WebClient();
                webClient.Encoding = Encoding.UTF8;
                result = webClient.DownloadString(Configuration.GetSection("GoogleMapLink").Value + latitude + "," + longitude + "&key=" + Configuration.GetSection("GeocodingAPIKey").Value);

                string regionName = "";
                string city = "";
                string countryName = "";

                locationInfo.Longitude = longitude;
                locationInfo.Latitude = latitude;

                if (String.IsNullOrEmpty(result) == false)
                {
                    GeocodingResponse geocodingResponse = JsonConvert.DeserializeObject<GeocodingResponse>(result);
                    if (geocodingResponse.results.Count > 0)
                    {
                        Dictionary<int, string> locationLevels = new Dictionary<int, string>();

                        locationLevels.Add(1, "neighborhood");
                        locationLevels.Add(2, "administrative_area_level_1");
                        locationLevels.Add(3, "locality");
                        locationLevels.Add(4, "administrative_area_level_2");
                        locationLevels.Add(5, "administrative_area_level_3");
                        locationLevels.Add(6, "route");
                        locationLevels.Add(7, "point_of_interest");

                        Dictionary<int, string> foundLocations = new Dictionary<int, string>();
                        int count = 1;

                        foreach (KeyValuePair<int, string> locationLevel in locationLevels)
                        {
                            string locationName = GetInfoByGeocodingAPI(geocodingResponse, locationLevel.Value, false);
                            if (String.IsNullOrEmpty(locationName) == false)
                            {
                                foundLocations.Add(count, locationName);
                                count += 1;
                            }
                        }

                        regionName = foundLocations.Count >= 1 ? foundLocations[1] : "";
                        city = foundLocations.Count >= 1 ? foundLocations[2] : "";
                        countryName = GetInfoByGeocodingAPI(geocodingResponse, "country", false);

                        List<String> address = new List<String>();
                        if (String.IsNullOrEmpty(countryName) == false)
                        {
                            address.Add(countryName);
                        }

                        if (String.IsNullOrEmpty(regionName) == false)
                        {
                            address.Add(regionName);
                        }

                        if (String.IsNullOrEmpty(city) == false)
                        {
                            address.Add(city);
                        }

                        locationInfo.RegionName = regionName;
                        locationInfo.City = city;
                        locationInfo.CountryName = countryName;
                        locationInfo.LocationAddress = String.Join(", ", address);
                    }
                }
                return locationInfo;
            }
            catch (Exception ex)
            {
                //_logger.Error(ex, "Location Error");
                LocationModal location = new LocationModal();
                return location;
            }
        }

        public string GetInfoByGeocodingAPI(GeocodingResponse geocodingResponse, string typeOfAddress, bool isShortName)
        {
            GeocodingAddressComponents geocodingAddressComponents
                            = geocodingResponse.results.First<GeocodingResponseResult>()
                            .address_components.Where(component => component.types.Contains(typeOfAddress)).FirstOrDefault();

            if (geocodingAddressComponents != null)
            {
                if (isShortName)
                {
                    return geocodingAddressComponents.short_name;
                }
                else
                {
                    return geocodingAddressComponents.long_name;
                }
            }
            else
            {
                return "";
            }
        }
    }
}
