using Application.Interface.Service;
using Application.ViewModels;
using AutoMapper.Internal.Mappers;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityPractice.Controllers
{
    public class LoginRegisterController : Controller
    {
        //private readonly IUserService _userService;
        //private readonly IRoleService _roleService;

        //public LoginRegisterController(IUserService userService, 
        //                                IRoleService roleService)
        //{
        //    _userService = userService;
        //    _roleService = roleService;
        //}
        //public async Task<IActionResult> Register( RegisterViewModel registerViewModel)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _userService.Register(registerViewModel);   
        //    }
        //    return View();
        //}
        //public async Task<IActionResult> Login (LoginViewModel loginViewModel)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _userService.Login(loginViewModel);
        //    }
        //    var user = _userService.GetUserByName(loginViewModel.UserName, loginViewModel.EmailAddress.ToString());
            
        //    return View(user);  

        //}
    }
}
