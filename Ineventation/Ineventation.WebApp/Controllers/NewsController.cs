using Ineventation.Business.Objects.Requests;
using Ineventation.WebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ineventation.WebApp.Controllers
{
    public class NewsController : BaseController
    {

        public NewsController() : base()
        {
        }

        public ActionResult ReadNews(string filter)
        {
            if (Session["userToken"] != null)
            {
                string token = Session["userToken"].ToString();
                var user = userService.LoadProfile(new LoadProfileObject { Token= token});
                var result = newsService.ReadNews(token, filter);
                return View(new NewsViewModel { list = result.Target, Filter = filter, MyId=user.Target.Id, AccountType= user.Target.AccountType});
            }
            return RedirectToAction("LogOut");
        }

        //public ActionResult Delete()
        //{
        //    if (Session["userToken"] != null)
        //    {
        //        var res = newsService.DeleteNews(Session["userToken"].ToString());
        //        if (res.Status)
        //        {
        //            return RedirectToAction("ReadNews");
        //        }
        //    }
        //    return RedirectToAction("LogOut");
        //}
    }
}