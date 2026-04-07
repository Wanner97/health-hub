using Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImportsController : ControllerBase
    {
        private readonly IStepsImportLogic _stepsImportLogic;

        public ImportsController(IStepsImportLogic stepsImportLogic)
        {
            _stepsImportLogic = stepsImportLogic;
        }

        [HttpPost("steps")]
        public IActionResult ImportSteps(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            using var stream = file.OpenReadStream();
            var result = _stepsImportLogic.ImportSteps(stream);

            return Ok(new
            {
                Message = "Steps imported successfully.",
                RecordCount = result.RecordCount
            });
        }
    }
}