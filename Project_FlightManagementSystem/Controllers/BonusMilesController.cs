using Project_FlightManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Project_FlightManagementSystem.Controllers
{
    [Authorize]
    public class BonusMilesController : Controller
    {
        // GET: BonusMiles
        public ActionResult Index()
        {
            return View();
        }

        CHN32_MMS124_TESTEntities db = new CHN32_MMS124_TESTEntities();
        // GET: Flight
        [HttpGet]
       [Authorize(Roles = "BonusMilesApprover")]
        public ActionResult AddBonusMiles()
        {
            ViewBag.list = getFlightName();
            return View();
        }
        public static SelectList getFlightName()
        {
            List<FMS_tbl_FlightScheduleDetails> list = new List<FMS_tbl_FlightScheduleDetails>();
            CHN32_MMS124_TESTEntities dbo = new CHN32_MMS124_TESTEntities();
            list = dbo.FMS_tbl_FlightScheduleDetails.ToList();


            return new SelectList(list, "FlightScheduleId", "Flight");


        }
       
        [HttpPost]
        public ActionResult AddBonusMiles(FormCollection fc, string create)
        {
            FMS_tbl_BonusMilesDEtails obj = new FMS_tbl_BonusMilesDEtails();
            if (create == null)
            {
                int distance = 0;
                obj.FlightScheduleId = int.Parse(fc["FlightScheduleId"].ToString());
                List<FMS_tbl_FlightScheduleDetails> list = db.FMS_tbl_FlightScheduleDetails.ToList();
                foreach (FMS_tbl_FlightScheduleDetails items in list)
                {
                    if (items.FlightScheduleId.Equals(obj.FlightScheduleId))
                    {
                        distance = (int)items.Distance;
                    }
                }
                ViewBag.list = getFlightName();
                obj.BonusMiles = Convert.ToInt32(distance * 0.25);
                TempData["BonusMiles"] = obj.BonusMiles;
                return View(obj);

            }
            else
            {
                try
                {
                    obj.FlightScheduleId = int.Parse(fc["FlightScheduleId"].ToString());
                    obj.BonusMiles = int.Parse(fc["BonusMiles"].ToString());
                }
                catch (Exception)
                {
                    ViewBag.Messag = "Flight Name or Bonus Miles Shouldn't be Empty.";
                    ViewBag.list = getFlightName();
                    return View();
                }
                
                List<FMS_tbl_BonusMilesDEtails> li= db.FMS_tbl_BonusMilesDEtails.ToList();
                foreach (FMS_tbl_BonusMilesDEtails item in li)
                {
                    if (obj.FlightScheduleId.Equals(item.FlightScheduleId))
                    {
                        ViewBag.Msg = "Already Added Miles For this Flight";
                        ViewBag.list = getFlightName();
                        return View();
                    }

                }
                
                if (obj.BonusMiles > (int)TempData["BonusMiles"])
                {
                    ViewBag.Msg = "Value of Bonus Miles should be below the value which is populated.";
                    ViewBag.list = getFlightName();
                    return View();
                }
                else
                {
                    db.FMS_tbl_BonusMilesDEtails.Add(obj);
                    db.SaveChanges();
                    int id = obj.BonusMilesId;
                    if (id > 1)
                    {
                        ViewBag.Msg = "Miles added Sucessfully";
                        ViewBag.list = getFlightName();
                        return View();
                    }
                    else
                    {
                        ViewBag.Msg = "Error while adding Miles";
                        ViewBag.list = getFlightName();
                        return View();
                    }
                }
                
            }
            


        }
        [Authorize(Roles = "BonusMilesApprover")]
        public ActionResult ViewBonusMiles()
        {
            List<FMS_tbl_BonusMilesDEtails> li = new List<FMS_tbl_BonusMilesDEtails>();
            li = db.FMS_tbl_BonusMilesDEtails.ToList();
            ViewBag.Message = TempData["Message"];
            return View(li);
        }
        [Authorize(Roles = "BonusMilesApprover")]
        public ActionResult ViewBonusMilesById()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult ViewBonusMilesById(FormCollection fc)
        {
            int id1 = int.Parse(fc["BonusMilesId"]);
            FMS_tbl_BonusMilesDEtails obj = new FMS_tbl_BonusMilesDEtails();
            obj = db.FMS_tbl_BonusMilesDEtails.FirstOrDefault(x => x.BonusMilesId == id1);
            if (obj != null)
            {
                return RedirectToAction("ViewDetails", new { id = id1 });
            }
            else
            {
                ViewBag.message = "Entered id is not correct";
                return View();
            }

        }
        [Authorize(Roles = "BonusMilesApprover")]
        public ActionResult ViewDetails(int id)
        {
            FMS_tbl_BonusMilesDEtails obj = new FMS_tbl_BonusMilesDEtails();
            obj = db.FMS_tbl_BonusMilesDEtails.Find(id);
            return View(obj);
        }
        [Authorize(Roles = "BonusMilesApprover")]
        public ActionResult DeleteBonusMilesById(int id)
        {
            TempData["id"] = id;
            FMS_tbl_BonusMilesDEtails obj = new FMS_tbl_BonusMilesDEtails();
            obj = db.FMS_tbl_BonusMilesDEtails.Find(id);
            return View(obj);
        }
        
        [HttpPost]
        public ActionResult DeleteBonusMilesById(FormCollection fc)
        {
            int id1 = (int)TempData["id"];
            FMS_tbl_BonusMilesDEtails obj = new FMS_tbl_BonusMilesDEtails();
            obj = db.FMS_tbl_BonusMilesDEtails.FirstOrDefault(x => x.BonusMilesId == id1);
            db.Entry(obj).State = EntityState.Deleted;
            try
            {

                int i = db.SaveChanges();
                TempData["Message"] = "Delete successful";
                return RedirectToAction("ViewBonusMiles");

            }
            catch (Exception)
            {
                ViewBag.messag = "Could not delete.";
                return View(obj);
            }

        }

        
    }
}