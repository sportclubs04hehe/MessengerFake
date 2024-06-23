using AutoMapper;
using MessengerFake.API.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace MessengerFake.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Route("api/[controller]")]
    [ApiController]
    public class BaseApiController : ControllerBase
    {
    }
}
