using Ineventation.Business.Services;
using System.Web.Mvc;

namespace Ineventation.WebApp.Controllers
{
    public class BaseController : Controller
    {
        protected UserService userService = null;
        protected CategoryService categoryService = null;
        protected EventService eventService = null;
        protected NewsService newsService = null;

        public BaseController()
        {
            userService = new UserService();
            eventService = new EventService();
            categoryService = new CategoryService();
            newsService = new NewsService();

        }

        public ActionResult LogOut()
        {
            if (Session["userToken"] != null)
            {
                Session["userToken"] = null;
            }
            return RedirectToAction("Index", "Home");

        }

    }
}