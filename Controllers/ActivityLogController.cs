using Application.Interface.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace IdentityPractice.Controllers
{
    public class ActivityLogController : Controller
    {
        private IActivityLogService activityLogService;
        public ActivityLogController(IActivityLogService activityLogService)
        {
            this.activityLogService = activityLogService;
        }
        public   IActionResult Index( string Id , string id2 )
        {
           var logs = activityLogService.getActivityLogOfUserOntaskId(Guid.Parse(Id), Guid.Parse(id2));
            return View(logs);
        }
    }
}
