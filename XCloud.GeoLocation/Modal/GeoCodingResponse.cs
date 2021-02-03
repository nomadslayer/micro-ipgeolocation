using System;
using System.Collections.Generic;
using System.Text;

namespace XCloud.GeoLocation.Modal
{
    public class GeocodingResponse
    {
        public List<GeocodingResponseResult> results { get; set; }
        public string status { get; set; }
    }

    public class GeocodingResponseResult
    {
        public List<GeocodingAddressComponents> address_components { get; set; }
    }

    public class GeocodingAddressComponents
    {
        public string long_name { get; set; }
        public string short_name { get; set; }
        public List<string> types { get; set; }
    }

    public class GeoLocationResponseResult : ServiceResponse
    {
        public LocationModal locationModal { get; set; }
    }
}
