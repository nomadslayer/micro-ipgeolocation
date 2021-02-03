using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using XCloud.GeoLocation.Filters;
using XCloud.GeoLocation.Modal;
using XCloud.GeoLocation.Service;

namespace XCloud.GeoLocation.Controllers
{
    [APIAuthorization]
    [ValidateModal]
    [EnableCors("AllowAll")]
    [Route("[controller]")]
    public class GeolocationController : BaseController
    {
		public GeolocationController(IConfiguration configuration, ILogger<GeolocationController> logger) : base(logger)
		{
			Configuration = configuration;
		}
		public IConfiguration Configuration { get; }

		[HttpPost("Location")]
		[ProducesResponseType(typeof(ServiceResponse), 200)]
		public IActionResult Location([FromBody] GeoLocationServiceRequest request)
		{
			if (request == null)
			{
				ServiceResponse response = new ServiceResponse
				{
					Success = false,
					Message = "No body provided"
				};
			}
			else if (request.IPAddress == string.Empty)
			{
				ServiceResponse response = new ServiceResponse
				{
					Success = false,
					Message = "No IP Address provided"
				};
			}
			else
			{
				GeoLocationService service = new GeoLocationService(Configuration);

				LocationModal locationModal = service.GetLocationByIPAddress(request.IPAddress);

				GeoLocationResponseResult geoLocationResponseResult = new GeoLocationResponseResult()
				{
					locationModal = locationModal,
					Success = true,
					Message = string.Empty
				};

				return Ok(geoLocationResponseResult);
			}

			return BadRequest("No response");
		}
	}
}
