using Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IHomepageDashboardLogic _homepageDashboardLogic;

        public DashboardController(IHomepageDashboardLogic homepageDashboardLogic)
        {
            _homepageDashboardLogic = homepageDashboardLogic;
        }

        [HttpGet("homepage")]
        public IActionResult GetHomepageDashboard()
        {
            var result = _homepageDashboardLogic.GetHomepageDashboard();

            return Ok(result);
        }
    }
}