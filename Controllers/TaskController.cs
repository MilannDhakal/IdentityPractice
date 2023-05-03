using Application.Interface.Data;
using Application.Interface.Service;
using Application.ViewModels.Tasks;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IdentityPractice.Controllers
{

    public class TaskController : Controller
    {
        private readonly IUserService _userService;
        private readonly ITasksService _tasksService;
        private readonly IRoleRepository _roleRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly IActivityLogService _activityLogService;
        private readonly ITasksUserService _tasksUserService;




        private readonly UserManager<User> _userManager;


        public TaskController(IActivityLogService activityLogService,
                                  IRoleRepository roleRepository,
                                  IUserService userService,
                                  ITasksService tasksService,
                                   ITaskRepository taskRepository,
                                   ITasksUserService tasksUserService,
            UserManager<User> userManager)
        {
            _activityLogService = activityLogService;
            _roleRepository = roleRepository;
            _userService = userService;
            _tasksService = tasksService;
            _taskRepository = taskRepository;
            _tasksUserService = tasksUserService;


            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            // Get Logedin User id

            var logedInUserId = _userManager.GetUserId(HttpContext.User);
            var role = _roleRepository.IsAdmin(Guid.Parse(logedInUserId));
            if (role is true)
            {
                var tasks = _taskRepository.GetAllTasks();
                return View(tasks);
            }
            else
            {
                var task = _taskRepository.GetTasksByUserId(Guid.Parse(logedInUserId));
                return View(task);
            }
        }

        public async Task<IActionResult> Details(string Id)
        {
           
            var details =  _taskRepository.GetTaskDetails(Guid.Parse(Id));
            return View(details);
        }
        
        public async Task<IActionResult> Create()
        {
            var claim = User.Claims.ToList();
            List<Guid> guidList = new List<Guid>();
            var emp = await _userService.GetEmployees();
            var listItem = new MultiSelectList
                (emp, "Id", "UserName");
            TaskCreateViewModel taskViewModel = new TaskCreateViewModel();
            taskViewModel.Employees = listItem;
            taskViewModel.EmployeesId = guidList;
            return View(taskViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaskCreateViewModel taskCreateViewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Model state is not valid");
                return View(taskCreateViewModel);
            }
           
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var result = await _tasksService.AddTaskAsync(taskCreateViewModel, user.Id);
            if (!result)
            {
                return View(taskCreateViewModel);
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Edit(string Id)
        {

            //--< from service
            var serviceEmp = await _tasksUserService.GetAssignedUserId(Id);
            var tasks = await _tasksService.GetTaskByIdAsync(Guid.Parse(Id));

            var emp = await _userService.GetEmployees();
            var listItem = new MultiSelectList
               (emp, "Id", "UserName");

            var taskViewModel = new TaskEditViewModel()
            {
                Id = tasks.Id,
                Name = tasks.Name,
                Description = tasks.Description,
                Employees = listItem,
                EmployeesId = serviceEmp,
                CreatedById = tasks.CreatedById
            };

            return View(taskViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TaskEditViewModel tasksViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(tasksViewModel);
            }
       
            var result = await _tasksService.UpdateTaskAsync(tasksViewModel);
            if(result== true)
            {
                return RedirectToAction(nameof(Index));
            }
            
            return View(tasksViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string Id)
        {
           

            var result = await _tasksService.DeleteTaskAsync(Guid.Parse(Id));
            if (result ==true)
            {
                return RedirectToAction("Index","Task");
            }
            return RedirectToAction("Index","Task");
        }


        public async Task<IActionResult> StartTask(string Id)
        {
           
            var logedInUserId = _userManager.GetUserId(HttpContext.User);
            var task = await _tasksService.GetTaskByIdAsync(Guid.Parse(Id));
            var tasksUser =await _tasksUserService.GetTasksUserByTaskIdAndUserId(Guid.Parse(logedInUserId), task.Id);
            if (tasksUser.Status== TasksUser.TaskStatus.Started)
            {
                //ModelState.AddModelError(string.Empty, "Task Is Started already, Please Stop or Pause");
                ViewData["ErrorMessage"] = "The Task is Started Already.";
                return RedirectToAction("Index", "Task");
            }
            if (tasksUser.Status == TasksUser.TaskStatus.Completed)
            {
                ModelState.AddModelError(string.Empty, "Task Is Completed");
                return RedirectToAction("Index", "Task");
            }
            if (task == null)
            {
                return NotFound();
            }
            var taskStart = await _tasksUserService.StartTask(Guid.Parse(Id), Guid.Parse(logedInUserId));
            if (taskStart is true)
            {
                return RedirectToAction("Index", "Task");
            }
            ModelState.AddModelError(string.Empty, "Cannot start task");
            return RedirectToAction("Index", "Task");

        }
        public async Task<IActionResult> PauseTask(string Id)
        {
            var logedInUserId = _userManager.GetUserId(HttpContext.User);
            var task = await _tasksService.GetTaskByIdAsync(Guid.Parse(Id));
            var tasksUser = await _tasksUserService.GetTasksUserByTaskIdAndUserId(Guid.Parse(logedInUserId), task.Id);
            if (tasksUser.Status == TasksUser.TaskStatus.Paused)
            {
                ModelState.AddModelError(string.Empty, "Task Is Started already, Please Stop or Pause");
                return RedirectToAction("Index", "Task");
            }
            if (tasksUser.Status == TasksUser.TaskStatus.NotStarted)
            {
                ModelState.AddModelError(string.Empty, "Task Is Completed");
                return RedirectToAction("Index", "Task");
            }
            if (tasksUser.Status == TasksUser.TaskStatus.Completed)
            {
                ModelState.AddModelError(string.Empty, "Task Is Completed");
                return RedirectToAction("Index", "Task");
            }
            if (task == null)
            {
                return NotFound();
            }
            var pauseTask = await _tasksUserService.PauseTask(Guid.Parse(Id), Guid.Parse(logedInUserId));
            if (pauseTask == true)
            {
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError(string.Empty, "cannot Pause task ");
            return RedirectToAction("Index", "Task");


        }

        public async Task<IActionResult> CompletedTask(string Id)
        {
            var logedInUserId = _userManager.GetUserId(HttpContext.User);

            var task = await _tasksService.GetTaskByIdAsync(Guid.Parse(Id));
            var tasksUser = await _tasksUserService.GetTasksUserByTaskIdAndUserId(Guid.Parse(logedInUserId), task.Id);
           
            if (tasksUser.Status == TasksUser.TaskStatus.NotStarted)
            {
                ModelState.AddModelError(string.Empty, "Task Is Not Started");
                return RedirectToAction("Index", "Task");
            }
            if (tasksUser.Status == TasksUser.TaskStatus.Completed)
            {
                ModelState.AddModelError(string.Empty, "Task Is Completed Already");
                return RedirectToAction("Index", "Task");
            }
            if (task == null)
            {
                return NotFound();
            }
            var stopTask = await _tasksUserService.StopTask(Guid.Parse(Id), Guid.Parse(logedInUserId));
            if (stopTask == true)
            {
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError(string.Empty, "Cannot Stop task ");
            return RedirectToAction("Index", "Task");
        }


    }
}
