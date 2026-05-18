using Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NutritionDaysController : ControllerBase
    {
        private readonly INutritionDayReadLogic _nutritionDayReadLogic;

        public NutritionDaysController(INutritionDayReadLogic nutritionDayReadLogic)
        {
            _nutritionDayReadLogic = nutritionDayReadLogic;
        }

        [HttpGet]
        public IActionResult GetNutritionDays([FromQuery] DateOnly? from, [FromQuery] DateOnly? to)
        {
            var result = _nutritionDayReadLogic.GetNutritionDays(from, to);

            return Ok(result);
        }
    }
}