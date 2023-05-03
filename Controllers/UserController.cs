//using Domain.Models;
//using Infrastructure.Data;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.EntityFrameworkCore;
//using System.Data;

//namespace IdentityPractice.Controllers
//{


//    public class UserController : Controller
//    {
//        private readonly ApplicationDbContext _db;
//        private readonly UserManager<User> _user;

//        public UserController(ApplicationDbContext db, UserManager<User> user)
//        {
//            _db = db;
//            _user = user;
//        }
//        public IActionResult Index()
//        {
//            var userList = _db.User.ToList();  //user
//            var userRole = _db.UserRoles.ToList(); //userRole
//            var roles = _db.Roles.ToList();         // role
//            //set user to none to not make ui look terrible
//            foreach (var user in userList)
//            {
//                var role = userRole.FirstOrDefault(u => u.UserId == user.Id);
//                if (role == null)
//                {
//                    user.Role = "None";
//                }
//                else
//                {
//                    user.Role = roles.FirstOrDefault(u => u.Id == role.RoleId).Name;
//                }
//            }

//            return View(userList);

//        }
//        [HttpGet]
//        public IActionResult Edit(Guid userId)
//        {
//            var user = _db.User.FirstOrDefault(u => u.Id == userId);
//            if (user == null)
//            {
//                return NotFound();
//            }
//            var userRole = _db.UserRoles.ToList();
//            var roles = _db.Role.ToList();
//            var role = userRole.FirstOrDefault(u => u.UserId == userId);
//            if (role != null)
//            {

//                user.RoleId = roles.FirstOrDefault(u => u.Id == role.RoleId)?.Id.ToString();

//            }
//            user.RoleList = _db.Role.Select(u => new SelectListItem
//            {
//                Text = u.Name,
//                Value = u.Id.ToString()
//            });
//            return View(user);
//        }
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Edit(User user) /// Error xa
//        {
//            if (ModelState.IsValid)
//            {
//                var userDbValue = _db.User.FirstOrDefault(u => u.Id == user.Id);
//                if (userDbValue == null)
//                {
//                    return NotFound();

//                }
//                var userRole = _db.UserRoles.FirstOrDefault(u => u.UserId == userDbValue.Id);
//                if (userRole != null)
//                {
//                    var previousRoleName = _db.Roles.Where(u => u.Id == userRole.RoleId).Select(u => u.Name).FirstOrDefault();
//                    await _user.RemoveFromRoleAsync(userDbValue, previousRoleName);
//                }
//                await _user.AddToRoleAsync(userDbValue, _db.Roles.FirstOrDefault(u => u.Id.Equals(user.RoleId)).Name);
//                _db.SaveChanges();
//                return RedirectToAction(nameof(Index));
//            }
//            user.RoleList = _db.Roles.Select(u => new SelectListItem
//            {
//                Text = u.Name,
//                Value = u.Id.ToString()
//            });
//            return View(user);
//        }

//        [HttpPost]
//        public async Task<IActionResult> Delete(Guid userId)
//        {
//            var user = await _user.FindByIdAsync(userId.ToString());
//            if (user == null)
//            {
//                return NotFound();
//            }
//            await _user.DeleteAsync(user);
//            _db.SaveChanges();
//            return RedirectToAction(nameof(Index));
//        }
//    }
//}
