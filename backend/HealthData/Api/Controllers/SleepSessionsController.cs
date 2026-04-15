using Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SleepSessionsController : ControllerBase
    {
        private readonly ISleepSessionReadLogic _sleepSessionReadLogic;

        public SleepSessionsController(ISleepSessionReadLogic sleepSessionReadLogic)
        {
            _sleepSessionReadLogic = sleepSessionReadLogic;
        }

        [HttpGet]
        public IActionResult GetSleepSessions([FromQuery] DateOnly? from, [FromQuery] DateOnly? to)
        {
            var result = _sleepSessionReadLogic.GetSleepSessions(from, to);
            return Ok(result);
        }
    }
}