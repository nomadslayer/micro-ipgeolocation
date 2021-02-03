using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace XCloud.GeoLocation.Controllers
{
    [Produces("application/json")]
    [Consumes("application/json")]
    //[EnableCors("AllowAll")]
    //[Route("api/v1/[controller]/[action]")]
    public class BaseController : Controller
    {
        protected readonly ILogger Logger;

        public BaseController(ILogger<BaseController> logger)
        {
            Logger = logger;
        }
    }
}
