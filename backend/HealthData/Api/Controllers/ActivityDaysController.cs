using Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActivityDaysController : ControllerBase
    {
        private readonly IActivityDayReadLogic _activityDayReadLogic;

        public ActivityDaysController(IActivityDayReadLogic activityDayReadLogic)
        {
            _activityDayReadLogic = activityDayReadLogic;
        }

        [HttpGet]
        public IActionResult GetActivityDays([FromQuery] DateOnly? from, [FromQuery] DateOnly? to)
        {
            var result = _activityDayReadLogic.GetActivityDays(from, to);
            return Ok(result);
        }
    }
}