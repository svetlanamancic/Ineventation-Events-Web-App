using System.Web;
using System.Web.Optimization;

namespace Ineventation.WebApp
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-3.4.1.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*", "~/Scripts/jquery.validate.unobtrusive.js"));
            

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/sendInvitations").Include("~/Scripts/SendInvitations.js"));
            bundles.Add(new ScriptBundle("~/bundles/nav").Include("~/Scripts/Nav.js"));
            bundles.Add(new ScriptBundle("~/bundles/maps").Include("~/Scripts/maps.js"));
            bundles.Add(new ScriptBundle("~/bundles/update").Include("~/Scripts/updateEvent.js"));



            bundles.Add(new ScriptBundle("~/bundles/modal").Include("~/Scripts/modal.js"));
            bundles.Add(new ScriptBundle("~/bundles/datepicker").Include("~/Scripts/DatePicker.js"));
            bundles.Add(new ScriptBundle("~/bundles/index").Include("~/Scripts/index.js"));
            bundles.Add(new ScriptBundle("~/bundles/signalr").Include("~/Scripts/signalr.js"));
            bundles.Add(new ScriptBundle("~/bundles/send").Include("~/Scripts/SendFriendRequest.js"));
            


            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/dark.css"));

        }
    }
}
