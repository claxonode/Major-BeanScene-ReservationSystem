using Major_BeanScene_ReservationSystem.Data.API_Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Major_BeanScene_ReservationSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;

        public UsersController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        // GET api/user/me
        [HttpGet("me")]
        public async Task<UserData> GetCurrentUser()
        {
            IdentityUser? user = await _userManager.GetUserAsync(User);
            
            // Not signed in
            if (user == null)
                return new UserData();
            return new UserData
            {
                Authorized = true,
                Username = user.UserName,
                Claims = (List<string>)await _userManager.GetRolesAsync(user),
            };
        }


    }
}
