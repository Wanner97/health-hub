using Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StepsController : ControllerBase
    {
        private readonly IStepsReadLogic _stepsReadLogic;

        public StepsController(IStepsReadLogic stepsReadLogic)
        {
            _stepsReadLogic = stepsReadLogic;
        }

        [HttpGet("latest")]
        public IActionResult GetLatestSteps()
        {
            var result = _stepsReadLogic.GetLatestSteps();

            return Ok(result);
        }
    }
}