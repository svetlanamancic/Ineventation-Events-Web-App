using Ineventation.WebApp.ViewModels;
using System.Web.Mvc;

namespace Ineventation.WebApp.Controllers
{
    public class HomeController : BaseController
    {

        public HomeController():base()
        {

        }

        public ActionResult Index()
        {
            var resCategories = categoryService.GetCategories().Target.Count;
            var resUsers = userService.GetUsers().Target.Count;
            var resEvents = eventService.GetEvents().Target.Count;

            return View(new LoginModel { eventNo =resEvents,userNo=resUsers,categoryNo=resCategories});
        }

        
    }
}