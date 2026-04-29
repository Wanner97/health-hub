using Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HeightMeasurementsController : ControllerBase
    {
        private readonly IHeightMeasurementReadLogic _heightMeasurementReadLogic;

        public HeightMeasurementsController(IHeightMeasurementReadLogic heightMeasurementReadLogic)
        {
            _heightMeasurementReadLogic = heightMeasurementReadLogic;
        }

        [HttpGet]
        public IActionResult GetHeightMeasurements(
            [FromQuery] DateTime? fromMeasuredAtUtc,
            [FromQuery] DateTime? toMeasuredAtUtc)
        {
            var result = _heightMeasurementReadLogic.GetHeightMeasurements(fromMeasuredAtUtc, toMeasuredAtUtc);

            return Ok(result);
        }
    }
}