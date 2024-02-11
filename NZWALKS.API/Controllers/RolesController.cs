using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Data;
using NZWALKS.API.Data;

namespace NZWALKS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly NZWalksAuthDbContext dbContext;
        
        [HttpGet]
        public IActionResult GetAllRoles()
        {
            string[] studentNames = new string[] { "John", "Jans" };
            return Ok(studentNames);
        }
    }
}
}
