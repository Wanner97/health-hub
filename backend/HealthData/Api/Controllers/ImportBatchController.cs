using Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImportBatchController : ControllerBase
    {
        private readonly IImportBatchLogic _importBatchLogic;

        public ImportBatchController(IImportBatchLogic importBatchLogic)
        {
            _importBatchLogic = importBatchLogic;
        }

        [HttpGet]
        public IActionResult GetImportBatches([FromQuery] DateOnly? from, [FromQuery] DateOnly? to)
        {
            var result = _importBatchLogic.GetImportBatches(from, to);

            return Ok(result);
        }
    }
}
