using Project_FlightManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Project_FlightManagementSystem.Controllers
{

    public class LoginController : Controller
    {
        CHN32_MMS124_TESTEntities db = new CHN32_MMS124_TESTEntities();
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Index(FMS_tbl_EmployeeLogin obj)
        {
            var count = db.FMS_tbl_EmployeeLogin.Where(x => x.FirstName == obj.FirstName && x.Pasword == obj.Pasword).Count();
            if (count == 0)
            {


                Response.Write("<script>alert('UserName or Password is Invalid');</script>");
                return View();
            }
            else
            {
                FormsAuthentication.SetAuthCookie(obj.FirstName, false);
                return RedirectToAction("Index", "Flight");
            }
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index");
        }




        public ActionResult About()
        {
            
            return View();
        }
        public ActionResult Contact()
        {
            
            return View();
        }
    }
}