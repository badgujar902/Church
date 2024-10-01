using Church.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Church.Areas.Individuals.Controllers
{
    public class IndividualsController : Controller
    {
        CHC_NewEntities dbcontext = new CHC_NewEntities();
        DateTime TodaysDate = DateTime.Now;
        string LoginMachinId = Dns.GetHostName();
        // GET: Individuals/Individuals
        public ActionResult Index()
        {
            return View();
        }
        #region MemberDashboard
        public ActionResult Dashboard()
        {
            try
            {
                if (Session["U_Fid"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }

                var currentDate = DateTime.Today;


                var GetNoticeData = dbcontext.Mas_Notice
                                          .Where(data => data.NoticeDateValid >= currentDate && data.Deactivate == false)
                                          .OrderByDescending(data => data.FId)
                                          .ToList();

                //var INdvslUserCurchId = Session["IndvslUserCurchId"];
                //int IndvslUserCurchId = Convert.ToInt32(INdvslUserCurchId);          
                //var NoticeList = dbcontext.sp_List_Notice(IndvslUserCurchId).ToList();

                if (GetNoticeData.Count() == 0 || GetNoticeData == null)
                {
                    ViewBag.NoticeData = "";
                }
                else
                {
                    ViewBag.NoticeData = GetNoticeData;
                }
                var eventImages = dbcontext.sp_List_Events().ToList();
                if (eventImages.Count() == 0 || eventImages == null)
                {
                    ViewBag.Events = "";
                }
                else
                {
                    ViewBag.Events = eventImages;
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
                var INdvslUserCurchId = Session["IndvslUserCurchId"];
                int IndvslUserCurchId = Convert.ToInt32(INdvslUserCurchId);
                var MemberFid = Session["U_Fid"];
                int IndvslFid = Convert.ToInt32(MemberFid);              
                //var GetNoticeNotification = dbcontext.sp_List_Notice(IndvslFid).ToList();
                var GetNoticeNotification = dbcontext.sp_List_Notice(IndvslFid).Where(x=>x.NoticeTillDate>= currentDate).ToList();
                var fIdsToRemove = new List<int>();
                var LeaderCreateNoticeFid = new List<int>();
                int flage = 0;
                int flage2 = 0;
                //var GetNoticeNotification1 = dbcontext.sp_List_Notice(IndvslUserCurchId).Where;

                foreach (var notice in GetNoticeNotification)
                {
                    var Seennotice = (from data in dbcontext.Mas_ReadView where data.NoticeFId == notice.FId && data.MemberFId == IndvslFid && data.Counter == 1 select data).FirstOrDefault();
                    if (Seennotice != null)
                    {
                        int NoticeID = Convert.ToInt32(Seennotice.NoticeFId);
                        flage = 1;
                        if (Seennotice != null)
                        {
                            fIdsToRemove.Add(NoticeID);
                        }
                    }
                    else
                    {
                        Session["NoticeNotificationData"] = GetNoticeNotification;

                        Session["NoticeNotificationCount"] = GetNoticeNotification.Count();
                    }
                }
                if (flage == 1)
                {
                    GetNoticeNotification.RemoveAll(notice => fIdsToRemove.Contains(notice.FId));

                    Session["NoticeNotificationData"] = GetNoticeNotification;

                    Session["NoticeNotificationCount"] = GetNoticeNotification.Count();
                }
                return View();
            }
            catch (Exception ex)
            {
                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }
        }
        #endregion    


        #region PrayerRequest
        public ActionResult PrayerRequest()
        {
            try
            {
                if (Session["U_Fid"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
                var MemberFId = Session["U_Fid"];
                int MemberFid = Convert.ToInt32(MemberFId);
                var MemberChurchId = (from data in dbcontext.MAS_INDVSL where data.FID == MemberFid select data).FirstOrDefault();
                var church = dbcontext.MAS_CHC.Where(X => X.Status == true && X.FID == MemberChurchId.MAS_CHC_FID).FirstOrDefault();
                ViewBag.ChurchList = church;
                return View();
            }
            catch (Exception ex)
            {
                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }
        }
        [HttpPost]
        public ActionResult SavePrayerRequest(MAS_PrayerReq Req)
        {
            var LoginMachinIp = Dns.GetHostByName(LoginMachinId).AddressList[0].ToString();
            try
            {
                if (Session["U_Fid"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
                var IndvslFID = Session["U_Fid"];
                var IndvslName = Session["IndividualName"];
                var IndvslChurchFid = Session["IndvslUserCurchId"];

                if (Session["IndividualName"] != null)
                {
                    MAS_PrayerReq Request = new MAS_PrayerReq();
                    Request.FDate = TodaysDate;
                    Request.Mac_ID = LoginMachinId;
                    Request.Mac_IP = LoginMachinIp;
                    Request.MAS_CHC_FID = Convert.ToInt32(IndvslChurchFid);
                    Request.Req_ID = Convert.ToInt32(IndvslFID);
                    Request.Req_Date = Req.Req_Date;
                    Request.PrayerRemark = Req.PrayerRemark;
                    Request.RequestType = Req.RequestType;
                    Request.Req_Status = false;
                    Request.Deactivate = false;
                    dbcontext.MAS_PrayerReq.Add(Request);
                    dbcontext.SaveChanges();
                    TempData["Message"] = "Prayer Request submitted successfully";
                    TempData["Icon"] = "success";
                }
                return RedirectToAction("PrayerRequest", "Individuals", new { area = "Individuals" });
            }
            catch (Exception ex)
            {
                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }
        }

        public ActionResult PrayerRequestList()
        {
            try
            {
                if (Session["U_Fid"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
                var MemberFid = Session["U_Fid"];
                int MemberFId = Convert.ToInt32(MemberFid);
                var MemberChurchId = Session["IndvslUserCurchId"];
                int MemberChurchFId = Convert.ToInt32(MemberChurchId);
                var today = DateTime.Today;

                //var GetprayerRequestsList = dbcontext.MAS_PrayerReq.Where(p => p.Req_ID == MemberFId && p.MAS_CHC_FID == MemberChurchFId && p.Deactivate == false && p.Req_Date>= today).OrderByDescending(p => p.FID).ToList();     
                var GetprayerRequestsList = dbcontext.MAS_PrayerReq.Where(p => p.Req_ID == MemberFId && p.MAS_CHC_FID == MemberChurchFId && p.Deactivate == false).OrderByDescending(p => p.FID).ToList();

                if (GetprayerRequestsList.Count == 0)
                {
                    ViewBag.ParayerRequestList = "";
                }
                else
                {
                    ViewBag.ParayerRequestList = GetprayerRequestsList;
                }
                return View();
            }
            catch (Exception ex)
            {
                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }
        }

        #endregion PrayerRequest

        #region UserProfile
        public ActionResult UserProfile()
        {
            try
            {
                if (Session["U_Fid"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }

                var INDVSLFid = Session["U_Fid"];
                int INDVSLFId = Convert.ToInt32(INDVSLFid);
                var ChurchData = (from data in dbcontext.MAS_UID where data.MAS_INDVSL_FID == INDVSLFId && data.U_Type == "Individual" select data).FirstOrDefault();
                int ChurchFid = Convert.ToInt32(ChurchData.MAS_CHC_FID);

                var GetMemberProfileData = dbcontext.sp_GetMemberData(INDVSLFId, ChurchFid).FirstOrDefault();
                ViewBag.MemberProfileData = GetMemberProfileData;


                //if (GetMemberProfileData != null)
                //{
                //    string ImageUrl = GetMemberProfileData.IND_Image;
                //    if (!Directory.Exists(ImageUrl))
                //    {
                //        try
                //        {
                //            using (Image image = Image.FromFile(ImageUrl))
                //            {
                //                using (MemoryStream m = new MemoryStream())
                //                {
                //                    image.Save(m, image.RawFormat);
                //                    byte[] imageBytes = m.ToArray();

                //                    // Convert byte[] to Base64 String
                //                    string base64String = Convert.ToBase64String(imageBytes);

                //                    var FinalPath = "data:image; base64," + base64String;
                //                    ViewBag.MemberProfileUrl = FinalPath;
                //                }
                //            }
                //        }
                //        catch (Exception ex)
                //        {
                //            ViewBag.MemberProfileUrl = "";
                //            return View();
                //        }

                //    }
                //}

                if (GetMemberProfileData != null)
                {
                    var BasUrl = (from data in dbcontext.DocumentBaseUrls select data).FirstOrDefault();
                    if (BasUrl != null)
                    {
                        string ImageUrl = GetMemberProfileData.IND_Image;
                        if (ImageUrl != null)
                        {
                            string fileName = Path.GetFileName(ImageUrl);
                            string baseUrl = BasUrl.BaseUrl;
                            string relativePath = ImageUrl.Replace("C:\\", "").Replace("\\", "/");
                            string finalImageUrl = baseUrl + relativePath;

                            ViewBag.MemberProfileUrl = finalImageUrl;
                            Session["MemberProfileUrl"] = finalImageUrl;
                        }
                        else
                        {
                            ViewBag.MemberProfileUrl = "";
                            Session["MemberProfileUrl"] = "";
                        }
                    }
                    else
                    {
                        ViewBag.MemberProfileUrl = "";
                        Session["MemberProfileUrl"] = "";
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
        #endregion


        #region NoticeList 
        public ActionResult NoticeList(string FromDate, string ToDate)
        {
            try
            {
                if (Session["U_Fid"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }


                var INdvslUserCurchId = Session["IndvslUserCurchId"];
                int IndvslUserCurchId = Convert.ToInt32(INdvslUserCurchId);

                var MemberFid = Session["U_Fid"];
                int IndvslFid = Convert.ToInt32(MemberFid);
                var fIdsToRemove = new List<int>();
                var LeaderCreateNoticeFid = new List<int>();
                int flage = 0;
                var NoticeList = dbcontext.sp_List_Notice(IndvslFid).ToList();
                var currentDate = DateTime.Today;

                foreach (var notice in NoticeList)
                {
                    var Seennotice = (from data in dbcontext.Mas_ReadView where data.NoticeFId == notice.FId && data.MemberFId == IndvslFid && data.Counter == 1 select data).FirstOrDefault();


                    if (currentDate > Convert.ToDateTime(notice.NoticeDateValid))
                    {
                        int NoticeID = Convert.ToInt32(notice.FId);
                        flage = 1;
                        fIdsToRemove.Add(NoticeID);

                    }
                    //if (Seennotice != null)
                    //{
                    //    int NoticeID = Convert.ToInt32(Seennotice.NoticeFId);
                    //    flage = 1;
                    //    if (Seennotice != null)
                    //    {
                    //        fIdsToRemove.Add(NoticeID);
                    //    }
                    //}
                    else
                    {
                        ViewBag.GetNoticeList = NoticeList;

                        //Session["NoticeNotificationCount"] = NoticeList.Count();
                    }
                }
                if (flage == 1)
                {
                    NoticeList.RemoveAll(notice => fIdsToRemove.Contains(notice.FId));

                    ViewBag.GetNoticeList = NoticeList;

                }

                if (FromDate != null && ToDate != null)
                {
                    DateTime fromdate = Convert.ToDateTime(FromDate);
                    DateTime Todate = Convert.ToDateTime(ToDate);
                    var GetNotice = (from data in dbcontext.Mas_Notice
                                     where data.NoticeDate >= fromdate
                                     && data.NoticeDate <= Todate
                                     && data.CurchId== IndvslUserCurchId
                                     && data.Deactivate == false
                                     orderby data.FId descending select data).ToList();

                    ViewBag.GetOldNoticeList = GetNotice;
                }


                return View();
            }
            catch (Exception ex)
            {
                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }
        }


        public ActionResult NoticeReadByMember(int? NoticeFID, int? DashboardView)
        {
            try
            {
                if (Session["U_Fid"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }

                var MemberFid = Session["U_Fid"];
                int MemberFId = Convert.ToInt32(MemberFid);

                var LoginmachinIp = Dns.GetHostByName(LoginMachinId).AddressList[0].ToString();
                Mas_ReadView ReadView = new Mas_ReadView();
                var ReadViewData = (from data in dbcontext.Mas_ReadView where data.NoticeFId == NoticeFID && data.MemberFId == MemberFId select data).FirstOrDefault();
                var MemberID = Session["U_Fid"];

                if (ReadViewData == null)
                {
                    ReadView.MacID = LoginMachinId;
                    ReadView.MacIP = LoginmachinIp;
                    ReadView.FDate = DateTime.Now;
                    ReadView.NoticeFId = NoticeFID;
                    ReadView.MemberFId = Convert.ToInt32(MemberID);
                    ReadView.Counter = 1;

                    dbcontext.Mas_ReadView.Add(ReadView);
                    dbcontext.SaveChanges();
                }
                if (DashboardView != null)
                {
                    return RedirectToAction("Dashboard", "Individuals", new { area = "Individuals" });
                }
                else
                {
                    return RedirectToAction("NoticeList", "Individuals", new { area = "Individuals" });
                }


            }
            catch (Exception ex)
            {

                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }
        }
        #endregion

        #region Enquery
        public ActionResult Enquiry()
        {
            try
            {
                if (Session["U_Fid"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
                return View();
            }
            catch (Exception ex)
            {
                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }
        }
        #endregion

        #region Enquiry Save
        [HttpGet]
        public ActionResult SaveEnquiry(string EnquirySubject, string Enquiry)
        {
            try
            {
                if (Session["U_Fid"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
                var LoginmachinIp = Dns.GetHostByName(LoginMachinId).AddressList[0].ToString();
                var MemberID = Session["U_Fid"];
                var MemberChurchFid = Session["IndvslUserCurchId"];
                Mas_Enquiry MasEnquiry = new Mas_Enquiry();
                int save = 0;
                if (EnquirySubject != null && Enquiry != null)
                {
                    MasEnquiry.MacID = LoginMachinId;
                    MasEnquiry.MacIP = LoginmachinIp;
                    MasEnquiry.FDate = DateTime.Now;
                    MasEnquiry.Subject = EnquirySubject;
                    MasEnquiry.Enquiry = Enquiry;
                    MasEnquiry.EnquiryDate = DateTime.Now;
                    MasEnquiry.CurchId = Convert.ToInt32(MemberChurchFid);
                    MasEnquiry.MemberFId = Convert.ToInt32(MemberID);
                    MasEnquiry.Status = true;
                    MasEnquiry.LeaderResponseStatus = true;
                    MasEnquiry.Deactivate = false;
                    dbcontext.Mas_Enquiry.Add(MasEnquiry);
                    save = dbcontext.SaveChanges();
                }

                if (save != 0)
                {
                    var Message = "Enquiry submitted successfully";
                    var Icon = "success";
                    return Json(new { Message, Icon }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var Message = " Enquiry Not submitted";
                    var Icon = "error";
                    return Json(new { Message, Icon }, JsonRequestBehavior.AllowGet);
                }


            }
            catch (Exception ex)
            {
                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }
        }
        #endregion

        #region EnquiryList 
        public ActionResult EnquiryList()
        {
            try
            {
                if (Session["U_Fid"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
                var MemberID = Session["U_Fid"];
                var MemberChurchFid = Session["IndvslUserCurchId"];
                int MemberFId = Convert.ToInt32(MemberID);
                int MemberChurchid = Convert.ToInt32(MemberChurchFid);

                var GetEnquiryList = (from data in dbcontext.Mas_Enquiry where data.CurchId == MemberChurchid && data.MemberFId == MemberFId && data.Deactivate == false orderby data.FId descending select data).ToList();
                ViewBag.EnquiryList = GetEnquiryList;

                if (GetEnquiryList.Count() == 0)
                {
                    ViewBag.EnquiryList = "";
                }
                else
                {
                    ViewBag.EnquiryList = GetEnquiryList;
                }
                return View();
            }
            catch (Exception ex)
            {
                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }
        }
        #endregion
    }
}