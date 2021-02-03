using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XCloud.GeoLocation.Modal
{
    public class GeoLocationServiceRequest
    {
        [Required(ErrorMessage = "IP is required.")]
        public string IPAddress { get; set; }
    }
}
