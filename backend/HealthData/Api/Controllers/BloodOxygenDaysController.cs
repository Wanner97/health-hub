using Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BloodOxygenDaysController : ControllerBase
    {
        private readonly IBloodOxygenDayReadLogic _bloodOxygenDayReadLogic;

        public BloodOxygenDaysController(IBloodOxygenDayReadLogic bloodOxygenDayReadLogic)
        {
            _bloodOxygenDayReadLogic = bloodOxygenDayReadLogic;
        }

        [HttpGet]
        public IActionResult GetBloodOxygenDays([FromQuery] DateOnly? from, [FromQuery] DateOnly? to)
        {
            var result = _bloodOxygenDayReadLogic.GetBloodOxygenDays(from, to);

            return Ok(result);
        }
    }
}