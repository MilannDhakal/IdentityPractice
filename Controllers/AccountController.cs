using Application.ViewModels;
using Domain.Models;
using Infrastructure.Data;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace IdentityPractice.Controllers
{
    public class AccountController : Controller
    {

        private readonly UserManager<User> _userManager; // For Adding user functionality
        private readonly RoleManager<Role> _roleManager;  // for Role Functionality  
        private readonly ApplicationDbContext _db;
        private readonly SignInManager<User> _signInManager; // For Signing in and displayig user

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<Role> roleManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            LoginViewModel loginViewModel = new LoginViewModel();
            loginViewModel.ReturnUrl = returnUrl ?? Url.Content("~/");
            return View(loginViewModel);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Model state is not valid");
                return View(loginViewModel);
            }

            var user = _db.User.FirstOrDefault(u => u.UserName == loginViewModel.UserName);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "The user does not exist");
                return View(loginViewModel);
            }

            var result = await _signInManager.PasswordSignInAsync(loginViewModel.UserName, loginViewModel.Password,
                         isPersistent: loginViewModel.RememberMe, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Cannot login");
                return View(loginViewModel);
            }

            //  var userRole = _db.UserRoles.FirstOrDefault(u => u.UserId == user.Id).RoleId;

            var role = (from ur in _db.UserRoles
                        join r in _db.Role on ur.RoleId equals r.Id
                        where ur.UserId == user.Id && r.IsAdmin == true
                        select r.IsAdmin).FirstOrDefault();

            if (role == true)
            {
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                return RedirectToAction("Index", "Employee");
            }
        }
        public async Task<IActionResult> Register(string? returnUrl = null)
        {

            var roles = await _db.Role.ToListAsync();
            var listItems = roles.Select(role => new SelectListItem
            {
                Value = role.Id.ToString(),
                Text = role.Name
            }).ToList();


            RegisterViewModel registerViewModel = new RegisterViewModel();
            registerViewModel.RoleList = listItems;
            registerViewModel.ReturnUrl = returnUrl;
            return View(registerViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel, String? returnUrl = null)
        {
            registerViewModel.ReturnUrl = returnUrl;
            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                // Check if the user already exists
                var existingUser = _db.Users.FirstOrDefault(u => u.UserName == registerViewModel.UserName || u.Email == registerViewModel.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError(string.Empty, "A user with the same username or email address already exists.");
                    return View(registerViewModel);
                }

                // Check if the selected role is valid
                var role = _db.Roles.FirstOrDefault(r => r.Id.ToString() == registerViewModel.RoleSelected);
                if (role == null)
                {
                    ModelState.AddModelError(string.Empty, "The selected role is invalid.");
                    return View(registerViewModel);
                }

                // Create the user
                var user = new User { UserName = registerViewModel.UserName, Email = registerViewModel.Email };
                var result = await _userManager.CreateAsync(user, registerViewModel.Password);
                if (result.Succeeded)
                {
                    // Assign the selected role to the user
                    var roleResult = await _userManager.AddToRoleAsync(user, role.ToString());
                    if (roleResult.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        await _userManager.DeleteAsync(user);
                        //  _db.Users.Remove(user);
                        foreach (var error in roleResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return View(registerViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }

}
