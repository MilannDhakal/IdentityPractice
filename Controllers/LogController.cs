using Domain.Models;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityPractice.Controllers
{
    public class LogController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<User> _userManager;

        public LogController(ApplicationDbContext db, UserManager<User> userManager)
        {
            _db = db;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index(string Id)
        {

            var task = _db.Task.FirstOrDefault(t => t.Id.ToString() == Id);
            var log = _db.ActivityLog.Where(t => t.Task.Id == task.Id).OrderByDescending(t => t.ActivityTime).ToList();

            return View(log);
        }
    }
}
