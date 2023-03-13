using Domain.Models;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityPractice.Controllers
{
    [Authorize]
    public class RoleController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public RoleController(ApplicationDbContext db, UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            var roles = _db.Role.ToList();
            return View(roles);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var role = await _db.Role.FindAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            return View(role);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Role role)
        {


            role.Id = Guid.NewGuid();
            // role.NormalizedName = role.Name.ToUpper();
            _db.Role.Update(role);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));


        }

        private bool RoleExists(Guid id)
        {
            return _db.Role.Any(e => e.Id == id);
        }
        public IActionResult Create() { return View(); }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(string id, Role role)
        {
            //      role.Id = Guid.NewGuid().ToString();
            role.NormalizedName = role.Name.ToUpper();

            _db.Roles.Add(role);
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));

           // return View(role);


        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(Role role)
        //{
        //    if (    _db.Roles.FirstOrDefault( == role.Name.ToString()); ;
        //    {
        //        return RedirectToAction("Index");
        //    }
        //    if (string.IsNullOrEmpty(role.Id))
        //    {
        //        await _roleManager.CreateAsync(new Role()
        //        {
        //            Name = role.Name,
        //            IsAdmin= role.IsAdmin,
        //        }) ;

        //    }
        //    else
        //    {
        //        var roleDb = _db.Role.FirstOrDefault(u => u.Id == role.Id);
        //       if (roleDb == null)
        //        {
        //            return RedirectToAction(nameof(Index));
        //        }
        //          roleDb.Name = role.Name; 
        //        roleDb.NormalizedName= role.Name.ToUpper();

        //        var result =  _db.Roles.Update(roleDb);

        //    }
        //    return RedirectToAction(nameof(Index));
        //}


    }
}
