using Church.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Church.Areas.Leader.Controllers
{
    public class MemberPendingEnquiryController : Controller
    {
        CHC_NewEntities dbcontext = new CHC_NewEntities();
        DateTime TodaysDate = DateTime.Now;
        string LoginMachinId = Dns.GetHostName();
        // GET: Leader/MemberPendingEnquiry
        public ActionResult MemberPendingEnquiry(int? id,int? ChurchFid)
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

                  
                    //var MemberPendingEnquiry = (from data in dbcontext.Mas_Enquiry where data.Status == true && data.MemberFId == id && data.LeaderResponseStatus == true && data.CurchId == LeaderChurchFId select data).ToList();
                    var MemberPendingEnquiry = dbcontext.sp_List_MemberPendingEnquiry(ChurchFid, id).ToList();
                    ViewBag.MemberPendingEnquiryRequest = MemberPendingEnquiry;


                    if (MemberPendingEnquiry.Count() == 0)
                    {
                        TempData["Message"] = "No Pending Inquiry ";
                        TempData["Icon"] = "error";
                        return RedirectToAction("ChurchMemberDetails", "ChurchMemberDetails", new { area = "Leader", MemberFId = id });
                    }
                }
                else
                {
                    if (Session["LeaderFId"] == null)
                    {
                        return RedirectToAction("Login", "Home", new { area = "" });
                    }
                    var LeaderChurchId = Session["LeaderCurchId"];
                    int LeaderChurchFId = Convert.ToInt32(LeaderChurchId);
                    //var MemberPendingEnquiry = (from data in dbcontext.Mas_Enquiry where data.Status == true && data.MemberFId == id && data.LeaderResponseStatus == true && data.CurchId == LeaderChurchFId select data).ToList();
                    var MemberPendingEnquiry = dbcontext.sp_List_MemberPendingEnquiry(LeaderChurchFId, id).ToList();
                    ViewBag.MemberPendingEnquiryRequest = MemberPendingEnquiry;


                    if (MemberPendingEnquiry.Count() == 0)
                    {
                        TempData["Message"] = "No Pending Inquiry ";
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