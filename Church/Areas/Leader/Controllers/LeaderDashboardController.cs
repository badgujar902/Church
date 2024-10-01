using Church.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Church.Areas.Leader.Controllers
{
    public class LeaderDashboardController : Controller
    {
        CHC_NewEntities dbcontext = new CHC_NewEntities();
        string LoginMachinId = Dns.GetHostName();
        // GET: Leader/LeaderDashboard
        public ActionResult PendingPrayerRequest()
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

                    var LeaderStateFid = (from data in dbcontext.MAS_LEADER where data.FID == HeadLeader_FId && data.Status == true select data).FirstOrDefault();

                    var PrayerRequest = dbcontext.PendingPrayerRequest(HeadLeaderCurchFId, LeaderDesignation.ToString(), null, LeaderStateFid.StateFid).ToList();
                    ViewBag.PendingPrayerRequest = PrayerRequest;
                    //if (PrayerRequest.Count == 0)
                    //{
                    //    TempData["Message"] = "No Pending Prayer Request";
                    //    TempData["Icon"] = "warning";
                    //    return RedirectToAction("Dashboard", "Leader", new { area = "Leader" });
                    //}
                }
                else
                {
                    if (Session["LeaderFId"] == null)
                    {
                        return RedirectToAction("Login", "Home", new { area = "" });
                    }
                    var LeaderFId = Session["LeaderFId"];
                    var curchFId = Session["LeaderCurchId"];
                    int LeaderFid = Convert.ToInt32(LeaderFId);
                    int LeaderChurchId = Convert.ToInt32(curchFId);

                    var PrayerRequest = dbcontext.PendingPrayerRequest(null, null, LeaderFid,null).ToList();
                    ViewBag.PendingPrayerRequest = PrayerRequest;
                    if (PrayerRequest.Count == 0)
                    {
                        TempData["Message"] = "No Pending Prayer Request";
                        TempData["Icon"] = "warning";
                        return RedirectToAction("Dashboard", "Leader", new { area = "Leader" });
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

        [HttpGet]
        public ActionResult PrayerRequestApprove(int? PrayerReqID, int? PrayerReqFID, string Comment)
        {
            try
            {
                var LeaderDesignation = Session["LeaderDesignation"];

                int  LeaderFId;

                if (LeaderDesignation.ToString() == "Head Leader")
                {
                    if (Session["HeadLeader_U_Fid"] == null)
                    {
                        return RedirectToAction("Login", "Home", new { area = "" });
                    }
                    var HeadLeaderFid = Session["HeadLeader_U_Fid"];
                    LeaderFId = Convert.ToInt32(HeadLeaderFid);
                }
                else
                {
                    if (Session["LeaderFId"] == null)
                    {
                        return RedirectToAction("Login", "Home", new { area = "" });
                    }
                   var  LeaderFid = Session["LeaderFId"];
                    LeaderFId = Convert.ToInt32(LeaderFid);
                }


                var LoginmachinIp = Dns.GetHostByName(LoginMachinId).AddressList[0].ToString();


                var Update = (from data in dbcontext.MAS_PrayerReq where data.FID == PrayerReqFID select data).FirstOrDefault();
                Update.Req_Status = true;
                Update.ApprovedDateTime = DateTime.Now;
                int I = dbcontext.SaveChanges();
                int Save = 0;
                //var data1 = "";
                if (I != 0)
                {
                    Mas_LeaderComment LeaderComment = new Mas_LeaderComment();
                    LeaderComment.FDate = DateTime.Now;
                    LeaderComment.Mac_ID = LoginMachinId;
                    LeaderComment.Mac_IP = LoginmachinIp;
                    LeaderComment.LeaderFID = LeaderFId;
                    LeaderComment.Comments = Comment;
                    LeaderComment.ReqFId = PrayerReqID;

                    dbcontext.Mas_LeaderComment.Add(LeaderComment);
                    Save = dbcontext.SaveChanges();
                    if (Save != 0)
                    {
                        //data1 = "Paryer Request Approved Successfuly ";
                        TempData["Message"] = "Prayer Request Approved Successfully";
                        TempData["Icon"] = "success";
                        return Json(new { Message = TempData["Message"], Icon = TempData["Icon"] }, JsonRequestBehavior.AllowGet);

                    }

                }
                return View();

                //Session["comment"] = Comment;
            }
            catch (Exception ex)
            {

                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }

        }

        public ActionResult RegistrationList()
        {
            try
            {
                if (Session["HeadLeader_U_Fid"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }

                var LeaderFId = Session["HeadLeader_U_Fid"];
                var curchFId = Session["HeadLeaderCurchId"];
                int LeaderCurch_Id = Convert.ToInt32(curchFId);

                var GetRistrationList = (from data in dbcontext.MAS_INDVSL where data.Status == true && data.MAS_CHC_FID == LeaderCurch_Id && data.Enroll_Type == "Self" && data.Deactivate == false orderby data.FID descending select (data)).ToList();


                ViewBag.RistrationList = GetRistrationList;
                //if (GetRistrationList.Count() == 0)
                //{
                //    TempData["Message"] = "No Pending Registration Request";
                //    TempData["Icon"] = "warning";
                //}
                return View();
            }
            catch (Exception ex)
            {

                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }

        }

        [HttpGet]
        public ActionResult CreateUser(int? INDVSLFID, string UserName, string UserPassword)
        {
            try
            {

                if (Session["LeaderFId"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
                if (INDVSLFID != null)
                {
                    var Update = (from data in dbcontext.MAS_INDVSL where data.FID == INDVSLFID select data).FirstOrDefault();
                    Update.Status = false;
                    Update.IND_DOJ = DateTime.Now;
                    int update = dbcontext.SaveChanges();
                    var LeaderFId = Session["LeaderFId"];
                    var CurchFID = Session["LeaderCurchId"];
                    string LoginMachinIP = Dns.GetHostByName(LoginMachinId).AddressList[0].ToString();
                    MAS_UID MasUserid = new MAS_UID();
                    if (update > 0)
                    {
                        MasUserid.FDate = DateTime.Now;
                        MasUserid.MacID = LoginMachinId;
                        MasUserid.MacIP = LoginMachinIP;
                        MasUserid.MAS_INDVSL_FID = INDVSLFID;
                        MasUserid.U_Type = "Individual";
                        MasUserid.U_ID = UserName;
                        MasUserid.U_Pass = UserPassword;
                        MasUserid.U_Status = true;
                        MasUserid.MAS_LEADER_FID = Convert.ToInt32(LeaderFId);
                        MasUserid.MAS_CHC_FID = Convert.ToInt32(CurchFID);
                        dbcontext.MAS_UID.Add(MasUserid);
                        int i = dbcontext.SaveChanges();
                        if (i != 0)
                        {
                            TempData["Message"] = "Username And Password Created Successfully";
                            TempData["Icon"] = "success";
                            return Json(new { Message = TempData["Message"], Icon = TempData["Icon"] }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            TempData["Message"] = "Username And Password Not Created Successfully";
                            TempData["Icon"] = "error";
                            return Json(new { Message = TempData["Message"], Icon = TempData["Icon"] }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        TempData["Message"] = "Username And Password Not Created Successfully";
                        TempData["Icon"] = "error";
                        return Json(new { Message = TempData["Message"], Icon = TempData["Icon"] }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    TempData["Message"] = "Username And Password Not Created Successfully";
                    TempData["Icon"] = "error";
                    return Json(new { Message = TempData["Message"], Icon = TempData["Icon"] }, JsonRequestBehavior.AllowGet);
                }


            }
            catch (Exception ex)
            {
                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }

        }
        public ActionResult MemberList()
        {
            try
            {
                var LeaderDesignation = Session["LeaderDesignation"];

                if (LeaderDesignation.ToString() == "Head Leader")
                {
                    var HeadLeaderFId = Session["HeadLeader_U_Fid"];
                    var HeadLeadercurchFId = Session["HeadLeaderCurchId"];
                    int HeadLeaderCurchFId = Convert.ToInt32(HeadLeadercurchFId);
                    int HeadLeader_FId = Convert.ToInt32(HeadLeaderFId);

                    var LeaderStateFid = (from data in dbcontext.MAS_LEADER where data.FID == HeadLeader_FId && data.Status == true select data).FirstOrDefault();
                    var GetTotalMemberList = dbcontext.sp_List_CurchMember(HeadLeaderCurchFId, null, "Head Leader", LeaderStateFid.StateFid).ToList();

                    ViewBag.CurchMemberList = GetTotalMemberList;
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
                    var GetTotalMemberList = dbcontext.sp_List_CurchMember(null, LeaderFid, null,null).ToList();
                    ViewBag.CurchMemberList = GetTotalMemberList;

                }

                return View();

            }
            catch (Exception ex)
            {

                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }
        }


        public ActionResult DonationList(string Fromdate, string ToDate, string Clear)
        {
            try
            {
                if (Session["LeaderFId"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
                var curchFId = Session["LeaderCurchId"];
                int LeaderChurchId = Convert.ToInt32(curchFId);
                if (Clear != "clear")
                {
                    if (Fromdate != null && ToDate != null)
                    {
                        DateTime fromdate = Convert.ToDateTime(Fromdate);
                        DateTime toDate = Convert.ToDateTime(ToDate);
                        var verifiedDonationList = dbcontext.sp_List_Donation(0, LeaderChurchId, fromdate, toDate).ToList();

                        if (verifiedDonationList.Count() == 0 || verifiedDonationList == null)
                        {
                            ViewBag.DonationList = "";
                        }
                        else
                        {
                            ViewBag.DonationList = verifiedDonationList;
                        }

                        var BasUrl = (from data in dbcontext.DocumentBaseUrls select data).FirstOrDefault();
                        if (BasUrl != null)
                        {
                            ViewBag.BasUrl = BasUrl.BaseUrl;
                        }
                        else
                        {
                            ViewBag.BasUrl = "";
                        }
                    }
                    else
                    {
                        var verifiedDonationList = dbcontext.sp_List_Donation(0, LeaderChurchId, null, null).ToList();
                        ViewBag.DonationList = verifiedDonationList;
                        if (verifiedDonationList.Count() == 0 || verifiedDonationList == null)
                        {
                            ViewBag.DonationList = "";
                        }
                        else
                        {
                            ViewBag.DonationList = verifiedDonationList;
                        }

                        var BasUrl = (from data in dbcontext.DocumentBaseUrls select data).FirstOrDefault();
                        if (BasUrl != null)
                        {
                            ViewBag.BasUrl = BasUrl.BaseUrl;
                        }
                        else
                        {
                            ViewBag.BasUrl = "";
                        }
                    }
                }
                else
                {
                    var verifiedDonationList = dbcontext.sp_List_Donation(0, LeaderChurchId, null, null).ToList();

                    if (verifiedDonationList.Count() == 0 || verifiedDonationList == null)
                    {
                        ViewBag.DonationList = "";
                    }
                    else
                    {
                        ViewBag.DonationList = verifiedDonationList;
                    }

                    var BasUrl = (from data in dbcontext.DocumentBaseUrls select data).FirstOrDefault();
                    if (BasUrl != null)
                    {
                        ViewBag.BasUrl = BasUrl.BaseUrl;
                    }
                    else
                    {
                        ViewBag.BasUrl = "";
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