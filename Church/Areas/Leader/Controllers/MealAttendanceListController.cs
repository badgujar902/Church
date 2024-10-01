using Church.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Church.Areas.Leader.Controllers
{
    public class MealAttendanceListController : Controller
    {
        CHC_NewEntities dbcontext = new CHC_NewEntities();
        DateTime TodaysDate = DateTime.Now;
        string LoginMachinId = Dns.GetHostName();
        // GET: Leader/MealAttendanceList
        public ActionResult MealAttendanceList()
        {
            try
            {
                var LeaderDesignation = Session["LeaderDesignation"];
                

                if (LeaderDesignation.ToString() == "Head Leader")
                {
                    if (Session["HeadLeader_U_Fid"] == null)
                    {
                        return RedirectToAction("Login", "Home", new { area = "" });
                    }
                    var HeadLeaderCurchId = Session["HeadLeaderCurchId"];
                    int HeadLeaderCurchFId = Convert.ToInt32(HeadLeaderCurchId);
                    var headLeaderFid = Session["HeadLeader_U_Fid"];
                    int headLeaderFID = Convert.ToInt32(headLeaderFid);

                    var LeaderStateFid = (from data in dbcontext.MAS_LEADER where data.FID == headLeaderFID && data.Status == true select data).FirstOrDefault();
                    
                    var GetMealAtten = dbcontext.sp_List_MealAttendance(HeadLeaderCurchFId, LeaderDesignation.ToString(), null, LeaderStateFid.StateFid).ToList();

                    ViewBag.MealAttendance = GetMealAtten;

                    //var totalAdults = dbcontext.MealAttendances.Sum(item => item.NoOFAdult);
                    //var totalChildren = dbcontext.MealAttendances.Sum(item => item.NoOfChild);

                    var totalAdults = GetMealAtten.Sum(item => item.NoOFAdult);

                    var totalChildren = GetMealAtten.Sum(item => item.NoOfChild);

                    var totalMealAttendance = totalAdults + totalChildren;
                    ViewBag.TotalAtten = totalMealAttendance;

                    ViewBag.TotalAdults = totalAdults;
                    ViewBag.TotalChildren = totalChildren;

                    ViewBag.TotalMealAttendance = totalAdults + totalChildren;

                }
                else
                {
                    if (Session["LeaderFId"] == null)
                    {
                        return RedirectToAction("Login", "Home", new { area = "" });
                    }
                    var LeaderFId = Session["LeaderFId"];
                    var curchFId = Session["LeaderCurchId"];
                    int LeaderCurch_Id = Convert.ToInt32(curchFId);
                    int LeaderFid = Convert.ToInt32(LeaderFId);
                    var GetMealAtten = dbcontext.sp_List_MealAttendance(LeaderCurch_Id, null, LeaderFid,null).ToList();
                    ViewBag.MealAttendance = GetMealAtten;

                    //var totalAdults = dbcontext.MealAttendances.Sum(item => item.NoOFAdult);
                    //var totalChildren = dbcontext.MealAttendances.Sum(item => item.NoOfChild);

                    var totalAdults = GetMealAtten.Sum(item => item.NoOFAdult);

                    var totalChildren = GetMealAtten.Sum(item => item.NoOfChild);

                    var totalMealAttendance = totalAdults + totalChildren;
                    ViewBag.TotalAtten = totalMealAttendance;
                    ViewBag.TotalAdults = totalAdults;
                    ViewBag.TotalChildren = totalChildren;

                    ViewBag.TotalMealAttendance = totalAdults + totalChildren;
                }                 

             
                return View();
            }
            catch (Exception ex)
            {

                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }
       
        }
    }
}