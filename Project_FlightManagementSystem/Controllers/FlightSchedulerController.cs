using Project_FlightManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project_FlightManagementSystem.Controllers
{
    [Authorize]
    public class FlightSchedulerController : Controller
    {
        // GET: FlightScheduler
        public ActionResult Index()
        {
            return View();
        }

        CHN32_MMS124_TESTEntities db = new CHN32_MMS124_TESTEntities(); 

        [HttpGet]

        [Authorize(Roles = "FlightScheduler")]
        public ActionResult AddFlightSchedule()
        {
            List<string> list = new List<string>() { "Delhi", "Kolkata", "Trivandrum", "Chennai", "Bangalore", "Mumbai" };
            ViewBag.ddlDeparturePlace = new SelectList(list);
            ViewBag.ddlDestinationPlace = new SelectList(list);
            ViewBag.ddlFlight = new SelectList(db.FMS_tbl_AirBusDetails, "Name", "Name");

            return View();
        }

        
        [HttpPost]
        public ActionResult AddFlightSchedule(string create, string Flight, FormCollection fc)
        {
            if (create == null)
            {


                if (Flight == "")
                {
                    ViewBag.ddlFlight = new SelectList(db.FMS_tbl_AirBusDetails, "Name", "Name");
                    string depName = fc["DeparturePlace"].ToString();
                    List<string> list = new List<string>() { "Delhi", "Kolkata", "Trivandrum", "Chennai", "Bangalore", "Mumbai" };
                    ViewBag.ddlDeparturePlace = new SelectList(list, depName);
                    List<string> li = new List<string>();

                    foreach (string item in list)
                    {
                        if (item.Equals(depName))
                            continue;
                        else
                            li.Add(item);
                    }
                    ViewBag.ddlDestinationPlace = new SelectList(li);

                    return View();
                }

                else
                {
                    FMS_tbl_FlightScheduleDetails obj = new FMS_tbl_FlightScheduleDetails();

                    obj.DestinationPlace = fc["DestinationPlace"].ToString();
                    obj.DeparturePlace = fc["DeparturePlace"].ToString();
                    List<string> list = new List<string>() { "Delhi", "Kolkata", "Trivandrum", "Chennai", "Bangalore", "Mumbai" };
                    ViewBag.ddlDestinationPlace = new SelectList(list, obj.DestinationPlace);
                    ViewBag.ddlDeparturePlace = new SelectList(list, obj.DeparturePlace);
                    ViewBag.ddlFlight = new SelectList(db.FMS_tbl_AirBusDetails, "Name", "Name");
                    obj.Flight = fc["Flight"].ToString();


                    List<FMS_tbl_AirBusDetails> lst1 = db.FMS_tbl_AirBusDetails.Where(e => e.Name == obj.Flight).ToList<FMS_tbl_AirBusDetails>();
                    foreach (FMS_tbl_AirBusDetails li in lst1)
                    {
                        obj.PremiumClassSeatingAvailability = li.PremiumClassSeatingCapacity;
                        obj.FirstClassSeatingAvailability = li.FirstClassSeatingCapacity;
                        obj.EconomyClassSeatingAvailability = li.EconomyClassSeatingCapacity;
                        obj.AirBusId = li.AirBusId;

                    }

                    return View(obj);
                }
            }
            else
            {
                if (ModelState.IsValid)
                {
                    FMS_tbl_FlightScheduleDetails obj = new FMS_tbl_FlightScheduleDetails();
                    try
                    {

                        obj.Flight = fc["Flight"].ToString();
                        obj.DeparturePlace = fc["DeparturePlace"].ToString();
                        obj.DestinationPlace = fc["DestinationPlace"].ToString();
                        obj.AirBusId = int.Parse(fc["AirBusId"].ToString());
                        obj.PremiumClassSeatingAvailability = int.Parse(fc["PremiumClassSeatingAvailability"].ToString());
                        obj.FirstClassSeatingAvailability = int.Parse(fc["FirstClassSeatingAvailability"].ToString());
                        obj.EconomyClassSeatingAvailability = int.Parse(fc["EconomyClassSeatingAvailability"].ToString());
                        obj.DepartureDate = DateTime.Parse(fc["DepartureDate"].ToString());
                        obj.ArrivalTime = DateTime.Parse(fc["ArrivalTime"].ToString());
                        obj.PremiumClassFare = int.Parse(fc["PremiumClassFare"].ToString());
                        obj.FirstClassFare = int.Parse(fc["FirstClassFare"].ToString());
                        obj.EconomyClassFare = int.Parse(fc["EconomyClassFare"].ToString());
                        obj.VAT = decimal.Parse(fc["VAT"].ToString());
                        obj.TAX = decimal.Parse(fc["TAX"].ToString());
                        obj.Distance = int.Parse(fc["Distance"].ToString());
                        obj.stats = true;

                        if (obj.DepartureDate > obj.ArrivalTime)
                        {
                            ViewBag.Messag = String.Format("Arrival Date Should not be below the Departure date.");
                            List<string> list = new List<string>() { "Delhi", "Kolkata", "Trivandrum", "Chennai", "Bangalore", "Mumbai" };
                            ViewBag.ddlDeparturePlace = new SelectList(list);
                            ViewBag.ddlDestinationPlace = new SelectList(list);
                            ViewBag.ddlFlight = new SelectList(db.FMS_tbl_AirBusDetails, "Name", "Name");
                            return View(obj);
                        }
                       
                            db.FMS_tbl_FlightScheduleDetails.Add(obj);
                    }
                    catch (Exception)
                    {
                        List<string> list = new List<string>() { "Delhi", "Kolkata", "Trivandrum", "Chennai", "Bangalore", "Mumbai" };
                        ViewBag.ddlDeparturePlace = new SelectList(list);
                        ViewBag.ddlDestinationPlace = new SelectList(list);
                        ViewBag.ddlFlight = new SelectList(db.FMS_tbl_AirBusDetails, "Name", "Name");
                        ViewBag.Messag = "Format You Entered is Incorrect.";
                        return View(obj);
                    }

                    try
                    {
                        db.SaveChanges();
                        int id = obj.FlightScheduleId;
                        if (id > 1)
                        {
                            ViewBag.Message = String.Format("Success: Flight Schedule added. Flight Schedule Id is {0}", id);
                            List<string> list = new List<string>() { "Delhi", "Kolkata", "Trivandrum", "Chennai", "Bangalore", "Mumbai" };
                            ViewBag.ddlDeparturePlace = new SelectList(list);
                            ViewBag.ddlDestinationPlace = new SelectList(list);
                            ViewBag.ddlFlight = new SelectList(db.FMS_tbl_AirBusDetails, "Name", "Name");
                            return View();
                        }
                        else
                        {
                            ViewBag.Message = "Error while adding Flight Schedule. Please try again";
                            List<string> list = new List<string>() { "Delhi", "Kolkata", "Trivandrum", "Chennai", "Bangalore", "Mumbai" };
                            ViewBag.ddlDeparturePlace = new SelectList(list);
                            ViewBag.ddlDestinationPlace = new SelectList(list);
                            ViewBag.ddlFlight = new SelectList(db.FMS_tbl_AirBusDetails, "Name", "Name");
                            return View();
                        }
                    } catch
                    {
                        List<string> list = new List<string>() { "Delhi", "Kolkata", "Trivandrum", "Chennai", "Bangalore", "Mumbai" };
                        ViewBag.ddlDeparturePlace = new SelectList(list);
                        ViewBag.ddlDestinationPlace = new SelectList(list);
                        ViewBag.ddlFlight = new SelectList(db.FMS_tbl_AirBusDetails, "Name", "Name");
                        ViewBag.Messag = "Flight is already Scheduled.";
                        return View(obj);
                    }
                
                }
                return View();
            }

        }
        [Authorize(Roles = "FlightScheduler")]
        public ActionResult EditFlightScheduling(int id)
        {
            FMS_tbl_FlightScheduleDetails obj = new FMS_tbl_FlightScheduleDetails();
            obj = db.FMS_tbl_FlightScheduleDetails.Find(id);
            return View(obj);
        }

        [HttpPost]
        public ActionResult EditFlightScheduling(FMS_tbl_FlightScheduleDetails obj)
        {
            try
            {
                if (obj.DepartureDate > obj.ArrivalTime)
                {
                    ViewBag.Messag = String.Format("Arrival Date Should not be below the Departure date.");
                    return View(obj);
                }
                db.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                ViewBag.Message = "Changes Saved Sucessfully";
                return View(obj);
            }
            catch
            {
                ViewBag.Messag = "Could not edit. Check values";
                return View(obj);
            }
           
        }




        [Authorize(Roles = "FlightScheduler")]
        public ActionResult ViewAllFlightScheduling()
        {
            List<FMS_tbl_FlightScheduleDetails> obj = new List<FMS_tbl_FlightScheduleDetails>();
            obj = db.FMS_tbl_FlightScheduleDetails.ToList();
            ViewBag.Message = TempData["Message"];
            return View(obj);
        }




        [Authorize(Roles = "FlightScheduler")]
        public ActionResult ViewFlightScheduleById()
        {
            return View();
        }
       
        [HttpPost]
        public ActionResult ViewFlightScheduleById(FormCollection fc)
        {
            int id = int.Parse(fc["FlightScheduleId"].ToString());
            if (db.FMS_tbl_FlightScheduleDetails.Any(x => x.FlightScheduleId.Equals(id)))
            {
                return RedirectToAction("DetailsOfFlightSchedule", new { id });
            }
            else
            {
                ViewBag.Messag = "INVALID Flight Schedule ID";
                return View();
            }
            
        }




        [Authorize(Roles = "FlightScheduler")]
        public ActionResult DetailsOfFlightSchedule(int id)
        {
            FMS_tbl_FlightScheduleDetails obj = new FMS_tbl_FlightScheduleDetails();
            obj = db.FMS_tbl_FlightScheduleDetails.Find(id);
            return View(obj);
        }




        [Authorize(Roles = "FlightScheduler")]
        public ActionResult DeleteFlightSchedule(int id)
        {

            FMS_tbl_FlightScheduleDetails obj = new FMS_tbl_FlightScheduleDetails();
            obj = db.FMS_tbl_FlightScheduleDetails.Find(id);
            return View(obj);
        }
       
        [HttpPost]
        public ActionResult DeleteFlightSchedule(FormCollection fc, int id)
        {

            FMS_tbl_FlightScheduleDetails obj = new FMS_tbl_FlightScheduleDetails();
            obj = db.FMS_tbl_FlightScheduleDetails.FirstOrDefault(x => x.FlightScheduleId == id);
            db.Entry(obj).State = System.Data.Entity.EntityState.Deleted;
            try
            {
                db.SaveChanges();
                TempData["Message"] = "Delete successful";
                return RedirectToAction("ViewAllFlightScheduling");
            }catch(Exception)
            {
                ViewBag.Messag = "Flight is already booked, Please release the tickets to continue.";
                return View(obj);
            }
                
            

        }

    }
}