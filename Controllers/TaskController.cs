using Application.ViewModels;
using Domain.Models;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System.ComponentModel;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace IdentityPractice.Controllers
{
    public class TaskController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;

        public TaskController(ApplicationDbContext db, RoleManager<Role> roleManager, UserManager<User> userManager)
        {
            _db = db;
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            // Get Logedin User id
            var logedInUserId = _userManager.GetUserId(HttpContext.User);
            // Find role
            var role = (from ur in _db.UserRoles
                        join r in _db.Role on ur.RoleId equals r.Id
                        where ur.UserId.ToString() == logedInUserId.ToString() && r.IsAdmin == true
                        select r.IsAdmin).FirstOrDefault();
            if (role == true)
            {
                var task = await _db.Task.ToListAsync();
                var userList = await _db.User.ToListAsync();
                foreach (var item in task)
                {
                    var user = userList.FirstOrDefault(u => u.Id == item.User.Id);
                    // var loginUser = userList.Contains(user.Id && user.Id== _userMan);
                }

                return View(task);

            }
            else
            {
                var tasks = await _db.Task.Where(t => t.User.Id.ToString() == logedInUserId).ToListAsync();

                return View(tasks);
            }

        }
        public IActionResult Create()
        {
            var query = from u in _db.Users
                        join ur in _db.UserRoles on u.Id equals ur.UserId
                        join r in _db.Role on ur.RoleId equals r.Id
                        where r.IsAdmin == false
                        select new { u.Id, u.UserName };
            // For dropdown in Add Task
            var listItem = query.Select(x => new SelectListItem()
            {
                Value = x.Id.ToString(),
                Text = x.UserName,

            });

            TasksViewModel taskViewModel = new TasksViewModel();
            taskViewModel.Employees = listItem.ToList();
            return View(taskViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TasksViewModel tasksViewModel)
        {
            if (!ModelState.IsValid)
            {
                // There are validation errors, so display them to the user
                foreach (var error in ModelState.Values)
                {
                    foreach (var err in error.Errors)
                    {
                        Console.WriteLine(err.ErrorMessage);
                    }
                }
            }
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id.ToString() == tasksViewModel.EmployeeSelected);
            var task = new Tasks
            {
                Id = Guid.NewGuid(),
                Name = tasksViewModel.Name,
                Description = tasksViewModel.Description,
                User = user
            };
            await _db.Task.AddAsync(task);
            var result = _db.SaveChanges();


            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> StartTask(string Id)
        {
            var task = _db.Task.FirstOrDefault(t => t.Id.ToString() == Id);
            //var taskName = task.Name;
            if (task == null)
            {
                return NotFound();
            }

            task.StartTask();


            //Insert Into Log
            ActivityLog activityLog = new()
            {
                Id = Guid.NewGuid(),
                Task = task,
                Action = Tasks.TaskStatus.Started.ToString()

            };
            _db.ActivityLog.Add(activityLog);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> PauseTask(string Id)
        {
            var task = await _db.Task.FirstOrDefaultAsync(t =>  t.Id.ToString() == Id);
            if (task == null)
            {
                return NotFound();
            }
            task.PauseTask();
            //        await _db.SaveChangesAsync();
            //Insert Into Log
            ActivityLog activityLog = new()
            {
               //Id = Guid.NewGuid(),
                Task = task,
                Action = Tasks.TaskStatus.Paused.ToString()
            };
            _db.ActivityLog.Add(activityLog);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> CompletedTask(string Id)
        {
            var task = await _db.Task.FirstOrDefaultAsync(t => t.Id.ToString() == Id);
            if (task == null)
            {
                return NotFound();
            }
            task.CompletedTask();
            // await _db.SaveChangesAsync();
            //Insert Into Log
            ActivityLog activityLog = new()
            {
              //  Id = Guid.NewGuid(),
                Task = task,
                Action = Tasks.TaskStatus.Completed.ToString()
            };
            await _db.ActivityLog.AddAsync(activityLog);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


    }
}
