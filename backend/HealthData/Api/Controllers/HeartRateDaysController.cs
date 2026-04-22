using Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HeartRateDaysController : ControllerBase
    {
        private readonly IHeartRateDayReadLogic _heartRateDayReadLogic;

        public HeartRateDaysController(IHeartRateDayReadLogic heartRateDayReadLogic)
        {
            _heartRateDayReadLogic = heartRateDayReadLogic;
        }

        [HttpGet]
        public IActionResult GetHeartRateDays(
            [FromQuery] DateOnly? from,
            [FromQuery] DateOnly? to,
            [FromQuery] bool includeHourlyRecords = false)
        {
            var result = _heartRateDayReadLogic.GetHeartRateDays(from, to, includeHourlyRecords);

            return Ok(result);
        }
    }
}