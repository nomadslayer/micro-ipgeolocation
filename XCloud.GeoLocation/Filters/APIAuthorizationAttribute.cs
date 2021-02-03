using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using XCloud.GeoLocation.Modal;

namespace XCloud.GeoLocation.Filters
{
	public class APIAuthorizationAttribute : TypeFilterAttribute
	{
		public APIAuthorizationAttribute() : base(typeof(APIAuthorizationFilter))
		{
		}

	}

	public class APIAuthorizationFilter : Attribute, IAuthorizationFilter
    {
		public APIAuthorizationFilter(IConfiguration configuration, ILogger<APIAuthorizationFilter> logger)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		public void OnAuthorization(AuthorizationFilterContext context)
		{
			string apiKey = context.HttpContext.Request.Headers["apiKey"].ToString();
			if (apiKey == "")
			{
				context.Result = new JsonResult(
					"Permission denied")
				{ StatusCode = 401 };
			}
			else
			{

				bool authorized = true;

				string key = Configuration.GetConnectionString("SecretKey");

				if (key == apiKey)
				{
					authorized = false;
				}

				if (authorized == false)
				{
					context.Result = new JsonResult(
						"Permission denied")
					{ StatusCode = 401 };
				}
			}
		}
	}
}
