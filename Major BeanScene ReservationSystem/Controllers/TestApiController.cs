using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.Claims;

namespace Major_BeanScene_ReservationSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestApiController : ControllerBase
    {
        

        [HttpGet("roles")]
        public IActionResult Roles()
        {
            HashSet<string> roles = new();

            foreach (string role in User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value))
            {
                roles.Add(role);
            }

            return Ok(roles);
        }

        [HttpGet("authorize")]
        [Authorize]
        public IActionResult Authorize()
        {
            return Ok();
        }

        [HttpGet("authorize-admin")]
        [Authorize]
        public IActionResult AuthorizeAdmin()
        {

            return Ok();
        }
        //Will redirect if not a staff
        [HttpGet("authorize-staff")]
        [Authorize(Roles = "Staff")]
        public IActionResult AuthorizeUser()
        {
            return Ok();
        }
    }
}
