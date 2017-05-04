using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PartyUp.Controllers
{
    /***
     * Used to display the homepage of the supporting view/website
     */
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //sets the title to hompage
            ViewBag.Title = "Home Page";

            //returns a view
            return View();
        }
    }
}
