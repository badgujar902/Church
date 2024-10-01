using Church.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Church.Areas.Leader.Controllers
{
    public class MemberTotalPrayerRequestController : Controller
    {
        CHC_NewEntities dbcontext = new CHC_NewEntities();
        DateTime TodaysDate = DateTime.Now;
        string LoginMachinId = Dns.GetHostName();
        // GET: Leader/MemberTotalPrayerRequest
        public ActionResult MemberTotalPrayerRequest(int? id)
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
                    var HeadLeaderFId = Session["HeadLeader_U_Fid"];
                    var HeadLeadercurchFId = Session["HeadLeaderCurchId"];
                    int HeadLeaderCurchFId = Convert.ToInt32(HeadLeadercurchFId);
                    int HeadLeader_FId = Convert.ToInt32(HeadLeaderFId);
                    
                    var MemberPrayerRequest = (from data in dbcontext.MAS_PrayerReq where data.Deactivate == false && data.Req_ID == id && data.Req_Status == false && data.MAS_CHC_FID == HeadLeaderCurchFId select data).ToList();

                    ViewBag.MemberPendingPrayerRequest = MemberPrayerRequest;
                    if (MemberPrayerRequest.Count() == 0)
                    {
                        TempData["Message"] = "No Pending Prayer Request ";
                        TempData["Icon"] = "error";
                        return RedirectToAction("ChurchMemberDetails", "ChurchMemberDetails", new { area = "Leader", MemberFId = id });
                    }
                }else
                {
                    if (Session["LeaderFId"] == null)
                    {
                        return RedirectToAction("Login", "Home", new { area = "" });
                    }
                    var LeaderChurchId = Session["LeaderCurchId"];
                    int LeaderChurchFId = Convert.ToInt32(LeaderChurchId);
                    var MemberPrayerRequest = (from data in dbcontext.MAS_PrayerReq where data.Deactivate == false && data.Req_ID == id && data.Req_Status == false && data.MAS_CHC_FID == LeaderChurchFId select data).ToList();

                    ViewBag.MemberPendingPrayerRequest = MemberPrayerRequest;
                    if (MemberPrayerRequest.Count() == 0)
                    {
                        TempData["Message"] = "No Pending Prayer Request ";
                        TempData["Icon"] = "error";
                        return RedirectToAction("ChurchMemberDetails", "ChurchMemberDetails", new { area = "Leader", MemberFId = id });
                    }
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