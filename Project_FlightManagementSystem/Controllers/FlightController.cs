using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project_FlightManagementSystem.Models;

namespace Project_FlightManagementSystem.Controllers
{
    [Authorize]
    public class FlightController : Controller
    {
        CHN32_MMS124_TESTEntities db = new CHN32_MMS124_TESTEntities();

        public ActionResult Index()
        {
            return View();
        }



        //dropdownlist method

        public SelectList getSeriesDetails()
        {
            List<FMS_tbl_AirBusSeries> list = new List<FMS_tbl_AirBusSeries>();
            list = db.FMS_tbl_AirBusSeries.ToList();
            return new SelectList(list, "Series", "Series");
        }


        //for adding airbus details
        [Authorize(Roles = "FlightScheduler")]
        public ActionResult AddAirBus()
        {
            ViewBag.DropDownList = getSeriesDetails();
            return View();
        }
        [HttpPost]
        public ActionResult AddAirBus(FormCollection fc, string Create)
        {
            if (Create == null)
            {
                FMS_tbl_AirBusDetails obj = new FMS_tbl_AirBusDetails();
                obj.Name = fc["Name"].ToString();
                obj.RegistrstionNumber = fc["RegistrstionNumber"].ToString();

                obj.AirBusSeries = fc["AirBusSeries"].ToString();
                List<FMS_tbl_AirBusSeries> list = db.FMS_tbl_AirBusSeries.ToList();


                ViewBag.DropDownList = getSeriesDetails();


                List<FMS_tbl_AirBusSeries> lst = db.FMS_tbl_AirBusSeries.Where(e => e.Series == obj.AirBusSeries).ToList<FMS_tbl_AirBusSeries>();
                foreach (FMS_tbl_AirBusSeries li in lst)
                {
                    obj.PremiumClassSeatingCapacity = li.PremiumClassSeatingCapacity;
                    obj.FirstClassSeatingCapacity = li.FirstClassSeatingCapacity;
                    obj.EconomyClassSeatingCapacity = li.EconomyClassSeatingCapacity;
                    obj.TakeOffWeight = li.MaxTakeOffWeight;
                    obj.MaximumFlightRange = li.MaxSpeed;
                }
                return View(obj);
            }
            else
            {
                if (ModelState.IsValid)
                {
                    FMS_tbl_AirBusDetails obj = new FMS_tbl_AirBusDetails();
                    obj.Name = fc["Name"].ToString();
                    obj.RegistrstionNumber = fc["RegistrstionNumber"].ToString();

                    if (db.FMS_tbl_AirBusDetails.Any(x => x.RegistrstionNumber.Equals(obj.RegistrstionNumber)))
                    {

                        ViewBag.Messag = String.Format("Sorry airbus details could not be added.Registration number already exists.");
                        ViewBag.DropDownList = getSeriesDetails();
                        return View();
                    }
                    else
                    {
                        obj.DateOfCommencement = DateTime.Parse(fc["DateOfCommencement"].ToString());
                        obj.ManufacturingDate = DateTime.Parse(fc["ManufacturingDate"].ToString());
                        obj.AirBusSeries = fc["AirBusSeries"].ToString();
                        obj.PremiumClassSeatingCapacity = int.Parse(fc["PremiumClassSeatingCapacity"].ToString());
                        obj.FirstClassSeatingCapacity = int.Parse(fc["FirstClassSeatingCapacity"].ToString());
                        obj.EconomyClassSeatingCapacity = int.Parse(fc["EconomyClassSeatingCapacity"].ToString());
                        obj.TakeOffWeight = decimal.Parse(fc["TakeOffWeight"].ToString());
                        obj.MaximumFlightRange = decimal.Parse(fc["MaximumFlightRange"].ToString());
                        if (obj.ManufacturingDate > obj.DateOfCommencement)
                        {
                            ViewBag.Messag = String.Format("Commencement Date Should not be below the Manufacturing date.");
                            ViewBag.DropDownList = getSeriesDetails();
                            return View(obj);
                        }
                        else
                        {
                            db.FMS_tbl_AirBusDetails.Add(obj);
                            try
                            {

                                db.SaveChanges();
                                int AirBusId = obj.AirBusId;

                                if (AirBusId > 1)
                                {
                                    ViewBag.Message = String.Format("Airbus details added successfully. AirBus Id is {0}", AirBusId);
                                    ViewBag.DropDownList = getSeriesDetails();
                                    return View();
                                }
                                else
                                {
                                    ViewBag.Messag = "Error while adding airbus. Please try again.";
                                    ViewBag.DropDownList = getSeriesDetails();
                                    return View();
                                }
                            }
                            catch (Exception)
                            {
                                ViewBag.Messag = "Flight Name already Exists.";
                                ViewBag.DropDownList = getSeriesDetails();
                                return View();
                            }
                        }
                    }
                }
            }
            return View();
        }


        //for viewing the entire list
        [Authorize(Roles = "FlightScheduler")]
        public ActionResult ViewAllAirBuses()
        {
            List<FMS_tbl_AirBusDetails> obj = new List<FMS_tbl_AirBusDetails>();
            obj = db.FMS_tbl_AirBusDetails.ToList();
            ViewBag.Message = TempData["Message"];
            return View(obj.OrderByDescending(x => x.DateOfCommencement));
        }


        //for editing the details
        [Authorize(Roles = "FlightScheduler")]
        public ActionResult EditAirBus(int id)
        {
            FMS_tbl_AirBusDetails obj = new FMS_tbl_AirBusDetails();
            obj = db.FMS_tbl_AirBusDetails.Find(id);
            return View(obj);
        }
        
        [HttpPost]
        public ActionResult EditAirBus(FMS_tbl_AirBusDetails obj, string Edit)
        {


            if (db.FMS_tbl_AirBusDetails.Any(x => x.Name.Equals(obj.Name)))
            {

                ViewBag.Messag = String.Format("Sorry airbus details could not be added.Name already exists.");

                return View(obj);
            }
            else
            {
                try
                {
                    db.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    ViewBag.Message = "Changes Saved Sucessfully";
                    return View(obj);
                }
                catch (Exception)
                {
                    ViewBag.Messag = "Flight Already Scheduled.";
                    return View(obj);
                }

            }
        }




        //for deleting the airbus details

        [Authorize(Roles = "FlightScheduler")]
        public ActionResult DeleteAirBus(int id)
        {

            FMS_tbl_AirBusDetails obj = new FMS_tbl_AirBusDetails();
            obj = db.FMS_tbl_AirBusDetails.Find(id);
            return View(obj);
        }
       
        [HttpPost]
        public ActionResult DeleteAirBus(FormCollection fc, int id)
        {
            FMS_tbl_AirBusDetails obj = new FMS_tbl_AirBusDetails();
            obj = db.FMS_tbl_AirBusDetails.FirstOrDefault(x => x.AirBusId == id);
            db.Entry(obj).State = System.Data.Entity.EntityState.Deleted;
            try
            {

                db.SaveChanges();
                TempData["Message"] = "Delete successful";
                return RedirectToAction("ViewAllAirBuses");

            }
            catch (Exception)
            {
                ViewBag.messag = "Could not delete.This flight is already scheduled.";
                return View(obj);
            }
        }



        //for editing the details by ID


        //public ActionResult EditById()
        //{
        //    return View();
        //}
        //[HttpPost]
        //public ActionResult EditById(FormCollection fc)
        //{

        //    int id = int.Parse(fc["AirBusId"].ToString());
        //    if (db.FMS_tbl_AirBusDetails.Any(x => x.AirBusId.Equals(id)))
        //    {
        //        return RedirectToAction("EditAirBus", new { id });
        //    }
        //    else
        //    {
        //        ViewBag.Messag = "INVALID AIRBUS ID";
        //        return View();
        //    }
        //}

        ////for deleting the details by ID 


        //public ActionResult DeleteById()
        //{
        //    return View();
        //}
        //[HttpPost]
        //public ActionResult DeleteById(FormCollection fc)
        //{

        //    int id = int.Parse(fc["AirBusId"].ToString());

        //    if (db.FMS_tbl_AirBusDetails.Any(x => x.AirBusId.Equals(id)))
        //    {
        //        return RedirectToAction("Delete", new { id });
        //    }
        //    else
        //    {
        //        ViewBag.Messag = "INVALID AIRBUS ID";
        //        return View();
        //    }

        //}

        //for viewing the details by ID
        [Authorize(Roles = "FlightScheduler")]
        public ActionResult ViewAirBusById()
        {
            return View();
        }
       
        [HttpPost]
        public ActionResult ViewAirBusById(FormCollection fc)
        {
            int id = int.Parse(fc["AirBusId"].ToString());

            if (db.FMS_tbl_AirBusDetails.Any(x => x.AirBusId.Equals(id)))
            {
                return RedirectToAction("Details", new { id });
            }
            else
            {
                ViewBag.Messag = "INVALID AIRBUS ID";
                return View();
            }
        }
        [Authorize(Roles = "FlightScheduler")]
        public ActionResult Details(int id)
        {
            FMS_tbl_AirBusDetails obj = new FMS_tbl_AirBusDetails();
            obj = db.FMS_tbl_AirBusDetails.Find(id);
            return View(obj);
        }

    }
}