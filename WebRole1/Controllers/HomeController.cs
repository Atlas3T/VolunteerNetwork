using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebRole1.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "What is the volunteer network?";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult DashboardJump()
        {
            if (User.IsInRole("Shopper"))
            {
                return RedirectToAction("Index", "Shopper");
            }
            else if (User.IsInRole("Volunteer"))
            {
                return RedirectToAction("Index", "Volunteer");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}