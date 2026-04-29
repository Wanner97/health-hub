using Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeightMeasurementsController : ControllerBase
    {
        private readonly IWeightMeasurementReadLogic _weightMeasurementReadLogic;

        public WeightMeasurementsController(IWeightMeasurementReadLogic weightMeasurementReadLogic)
        {
            _weightMeasurementReadLogic = weightMeasurementReadLogic;
        }

        [HttpGet]
        public IActionResult GetWeightMeasurements(
            [FromQuery] DateTime? fromMeasuredAtUtc,
            [FromQuery] DateTime? toMeasuredAtUtc)
        {
            var result = _weightMeasurementReadLogic.GetWeightMeasurements(fromMeasuredAtUtc, toMeasuredAtUtc);

            return Ok(result);
        }
    }
}