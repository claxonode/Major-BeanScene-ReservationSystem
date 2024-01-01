using Major_BeanScene_ReservationSystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Major_BeanScene_ReservationSystem.Areas.Member.Controllers
{
    [Area("Member")]
    [Authorize (Roles = "Member")]
    public class MemberBaseController : Controller
    {
        protected readonly ApplicationDbContext _context;
        public MemberBaseController(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}
