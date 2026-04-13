using Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImportsController : ControllerBase
    {
        private readonly IImportBatchLogic _importBatchLogic;

        public ImportsController(IImportBatchLogic importBatchLogic)
        {
            _importBatchLogic = importBatchLogic;
        }

        [HttpPost("activity")]
        public IActionResult ImportActivity(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            using (var stream = file.OpenReadStream())
            {
                var result = _importBatchLogic.ImportActivity(stream);

                return Ok(new
                {
                    Message = "Activity import completed successfully.",
                    ReceivedRecordCount = result.ReceivedRecordCount,
                    InsertedRecordCount = result.InsertedRecordCount,
                    UpdatedRecordCount = result.UpdatedRecordCount,
                    UnchangedRecordCount = result.UnchangedRecordCount
                });
            }
        }
    }
}