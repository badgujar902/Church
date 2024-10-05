using Church.Areas.Admin.Models;
using Church.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Church.Areas.Leader.Controllers
{
    public class LeaderController : Controller
    {
        CHC_NewEntities dbcontext = new CHC_NewEntities();
        DateTime TodaysDate = DateTime.Now;
        string LoginMachinId = Dns.GetHostName();

        // GET: Leader/Leader

        public ActionResult Index()
        {
            try
            {
                if (Session["LeaderFId"] == null)
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

        public ActionResult Dashboard()
        {
            try
            {
                var LeaderDesignation = Session["LeaderDesignation"];

                //var PrayerRequest = dbcontext.MAS_PrayerReq.Where(x => x.Req_Status == false && x.MAS_CHC_FID == LeaderCurch_Id && x.Req_ID != null).ToList();
                //TempData["PrayerRequestCount"] = PrayerRequest.Count();
                if(LeaderDesignation!=null)
                {
                    if (LeaderDesignation.ToString() == "Head Leader")
                    {
                        if (Session["HeadLeader_U_Fid"] == null)
                        {
                            return RedirectToAction("Login", "Home", new { area = "" });
                        }
                        var HeadLeaderCurchId = Session["HeadLeaderCurchId"];
                        int HeadLeaderCurchFId = Convert.ToInt32(HeadLeaderCurchId);
                        var headLeaderFid = Session["HeadLeader_U_Fid"];
                        int headLeaderFID= Convert.ToInt32(headLeaderFid);

                        var LeaderStateFid = (from data in dbcontext.MAS_LEADER where data.FID == headLeaderFID && data.Status == true select data).FirstOrDefault();
                        var PrayerRequest = dbcontext.PendingPrayerRequest(HeadLeaderCurchFId, LeaderDesignation.ToString(), null, LeaderStateFid.StateFid).ToList();
                        TempData["PrayerRequestCount"] = PrayerRequest.Count();

                        var GetResitration = dbcontext.MAS_INDVSL.Where(x => x.Status == true && x.MAS_CHC_FID == HeadLeaderCurchFId && x.StateFid== LeaderStateFid.StateFid && x.Enroll_Type == "Self" && x.Deactivate == false).ToList();
                        TempData["GetResitrationRequestCount"] = GetResitration.Count();

                        var GetTotalMemberList = dbcontext.sp_List_CurchMember(HeadLeaderCurchFId, null, "Head Leader", LeaderStateFid.StateFid).ToList();
                        TempData["TotalMeberList"] = GetTotalMemberList.Count();

                        var GetEnquiry = dbcontext.sp_List_Enquiry(HeadLeaderCurchFId, LeaderDesignation.ToString(), null, LeaderStateFid.StateFid).ToList();
                        TempData["TotalEnquiryList"] = GetEnquiry.Count();

                        //var MealAttendance = dbcontext.MealAttendances(HeadLeaderCurchFId, LeaderDesignation.ToString(), null)

                        var GetMealAtten = dbcontext.sp_List_MealAttendance(HeadLeaderCurchFId, LeaderDesignation.ToString(), null, LeaderStateFid.StateFid).ToList();
                        int TotalMEalAtten = 0;
                        foreach (var item in GetMealAtten)
                        {
                            TotalMEalAtten = TotalMEalAtten + Convert.ToInt32(item.NoOFAdult);
                            TotalMEalAtten = TotalMEalAtten + Convert.ToInt32(item.NoOfChild);
                        }
                        TempData["GetGetMealAttendance"] = TotalMEalAtten;

                        //Show Event 
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

                        //Show Event 
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

                        var PrayerRequest = dbcontext.PendingPrayerRequest(null, null, LeaderFid,null).ToList();
                        TempData["PrayerRequestCount"] = PrayerRequest.Count();

                        var GetResitration = dbcontext.MAS_INDVSL.Where(x => x.Status == true && x.MAS_CHC_FID == LeaderCurch_Id && x.Enroll_Type == "Self" && x.Deactivate == false).ToList();
                        TempData["GetResitrationRequestCount"] = GetResitration.Count();

                        var GetTotalMemberList = dbcontext.sp_List_CurchMember(null, LeaderFid, null,null).ToList();
                        TempData["TotalMeberList"] = GetTotalMemberList.Count();


                        var GetEnquiry = dbcontext.sp_List_Enquiry(null, null, LeaderFid,null).ToList();
                        TempData["TotalEnquiryList"] = GetEnquiry.Count();

                        var ChurchDonationList = dbcontext.sp_List_Donation(0, LeaderCurch_Id, null, null).ToList();
                        TempData["DonationList "] = ChurchDonationList.Count();

                        var GetMealAtten = dbcontext.sp_List_MealAttendance(LeaderCurch_Id, null, LeaderFid,null).ToList();
                        int TotalMEalAtten = 0;
                        foreach (var item in GetMealAtten)
                        {
                            TotalMEalAtten = TotalMEalAtten + Convert.ToInt32(item.NoOFAdult);
                            TotalMEalAtten = TotalMEalAtten + Convert.ToInt32(item.NoOfChild);
                        }
                        TempData["GetGetMealAttendance"] = TotalMEalAtten;

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
                        var GetNoticeNotification = dbcontext.sp_List_Notice(LeaderCurch_Id).ToList();
                        var fIdsToRemove = new List<int>();
                        int flage = 0;
                        foreach (var notice in GetNoticeNotification)
                        {
                            var Seennotice = (from data in dbcontext.Mas_ReadView where data.NoticeFId == notice.FId && data.MemberFId == LeaderFid && data.Counter == 1 select data).FirstOrDefault();

                            int NoticeID;
                            if (notice.Meal_Attandence == true)
                            {
                                //NoticeID = Convert.ToInt32(Seennotice.NoticeFId);
                                flage = 1;
                               
                               fIdsToRemove.Add(notice.FId);                                
                            }
                           else if (Seennotice != null)
                            {
                                 NoticeID = Convert.ToInt32(Seennotice.NoticeFId);
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
                    }
                }else
                {                    
                        return RedirectToAction("Login", "Home", new { area = "" });                 
                }
                //var MealAttendance = dbcontext.sp_List_MealAttendance(LeaderChurchId).ToList();
                //TempData["MealAttendance "] = MealAttendance.Count();

                return View();
            }
            catch (Exception ex)
            {
                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }

        }

        #region Individual Create By Leader 
        public ActionResult IndividualCreation()
        {
            try
            {
                var LeaderDesignation = Session["LeaderDesignation"];
              
                if (Session["HeadLeader_U_Fid"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
                //Thread.CurrentThread.CurrentCulture = new CultureInfo("ar-SA");
                //ViewBag.Date = DateTime.Now.ToLongDateString();
                var HeadLeaderFId = Session["HeadLeader_U_Fid"];
                var HeadLeadercurchFId = Session["HeadLeaderCurchId"];
                int HeadLeaderCurchFId = Convert.ToInt32(HeadLeadercurchFId);
                int HeadLeader_FId = Convert.ToInt32(HeadLeaderFId);

                var GetState = (from data in dbcontext.MAS_STATE where data.Active == 1 select new BindDrop { Id = data.Fid, Name = data.State }).ToList();
                ViewBag.GEtStateName = GetState;
                //var LeaderchurchId = dbcontext.MAS_UID.Where(X => X.MAS_LEADER_FID == LeaderFid).FirstOrDefault();
                //var church = dbcontext.MAS_CHC.Where(X => X.Status == true && X.FID == LeaderchurchId.MAS_CHC_FID).FirstOrDefault();
                var LeaderchurchId = dbcontext.MAS_UID.Where(X => X.MAS_LEADER_FID == HeadLeader_FId).FirstOrDefault();
                var church = dbcontext.MAS_CHC.Where(X => X.Status == true && X.FID == LeaderchurchId.MAS_CHC_FID).FirstOrDefault();
                ViewBag.ChurchList = church;
                //GetChurchList();
                return View();
            }
            catch (Exception ex)
            {
                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }

        }

        [HttpGet]
        public ActionResult IsExistsData(string Email, string MobileNumber)
        {
            try
            {

                string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
                if (Email != "")
                {
                    if (!Regex.IsMatch(Email, pattern))
                    {
                        TempData["Message"] = "Enter Valid Email Address !";
                        TempData["Icon"] = "error";
                        return Json(new { Message = TempData["Message"], Icon = TempData["Icon"] }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                int error = 0;
                if (Email != "")
                {
                    //var Data = dbcontext.MAS_INDVSL.Where(X => X.AdharNo == AdharNumber).FirstOrDefault();
                    var GetEmail = dbcontext.MAS_INDVSL.Where(X => X.IND_Email == Email && X.Deactivate==false).FirstOrDefault();
                    var GetMobileNumber = dbcontext.MAS_INDVSL.Where(X => X.IND_Mob == MobileNumber && X.Deactivate==false).FirstOrDefault();

                    //if (Data != null)
                    //{
                    //    if (Data.AdharNo != null)
                    //    {
                    //        error = 1;
                    //        TempData["Message"] = "Aadhar Number Can not be Duplicate";
                    //        TempData["Icon"] = "error";
                    //        return Json(new { Message = TempData["Message"], Icon = TempData["Icon"] }, JsonRequestBehavior.AllowGet);
                    //    }
                    //}
                    if (GetEmail != null)
                    {
                        if (GetEmail.IND_Email != null)
                        {
                            error = 1;
                            TempData["Message"] = "Email Already Exist";
                            TempData["Icon"] = "error";
                            return Json(new { Message = TempData["Message"], Icon = TempData["Icon"] }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    if (GetMobileNumber != null)
                    {
                        if (GetMobileNumber.IND_Mob != null)
                        {
                            error = 1;
                            TempData["Message"] = "Mobile Number Can not be Duplicate";
                            TempData["Icon"] = "error";
                            return Json(new { Message = TempData["Message"], Icon = TempData["Icon"] }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                else
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                if (error != 0)
                {
                    return Json(new { Message = TempData["Message"], Icon = TempData["Icon"] }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }
        }
        [HttpPost]
        public ActionResult SaveIndividualCreation(Individual individual1, HttpPostedFileBase file)
        {
            string LoginmachinIp = Dns.GetHostByName(LoginMachinId).AddressList[0].ToString();
            try
            {
                int save = 0;
                var LeaderFid = Session["HeadLeader_U_Fid"];
                int LeaderFID = Convert.ToInt32(LeaderFid);

              
                var HeadLeaderCurchId = Session["HeadLeaderCurchId"];
                int HeadLeaderCurchFId = Convert.ToInt32(HeadLeaderCurchId);

                HttpPostedFile files = System.Web.HttpContext.Current.Request.Files["file"];

                var GetChrurchArea = (from data in dbcontext.MAS_LEADER where data.FID == LeaderFID && data.Status == true select data).FirstOrDefault();
                var ChurchFid = Session["LeaderCurchId"];
                if (Session["HeadLeader_U_Fid"] != null)
                {
                    if (individual1 != null)
                    {
                        MAS_INDVSL individual = new MAS_INDVSL();
                        individual.FDate = TodaysDate;
                        individual.MacID = LoginMachinId;
                        individual.MacIP = LoginmachinIp;
                        individual.MAS_CHC_FID = individual1.MAS_CHC_FID;
                        individual.IND_Name = individual1.IND_Name;
                        individual.IND_Mob = individual1.IND_Mob;
                        individual.IND_Email = individual1.IND_Email;
                        individual.IND_Address = individual1.IND_Address;
                        individual.IND_DOJ = DateTime.Now;
                        individual.Gender = individual1.Gender;
                        individual.IND_DOB = individual1.IND_DOB;
                        //individual.StateFid = GetChrurchArea.StateFid;
                        //individual.PostCode = GetChrurchArea.PostCode;
                        //individual.AdharNo = individual1.MemberAdharNo;
                        individual.Enroll_Type = "Head Leader";
                        var Enroll_ID = ((from db in dbcontext.MAS_INDVSL select (int?)db.Enroll_ID).Max() ?? 0) + 1;
                        individual.Enroll_ID = Enroll_ID;
                        //individual.Status = true;
                        individual.StateFid = individual1.StateFid;
                        individual.City = individual1.City;
                        individual.Area = individual1.Area;
                        individual.Status = false;
                        individual.Deactivate = false;

                        dbcontext.MAS_INDVSL.Add(individual);
                        save = dbcontext.SaveChanges();

                        var filelctn = individual.IND_Mob;
                        string MoveLocation = @"C:\StealthChurch\RegistrationData\\" + filelctn + "\\";

                        if (files.ContentLength > 0)
                        {
                            if (!Directory.Exists(MoveLocation))
                            {
                                Directory.CreateDirectory(MoveLocation);
                            }
                            files.SaveAs(MoveLocation + Path.GetFileName((files.FileName)));
                            individual.IND_Image = Convert.ToString(MoveLocation + Path.GetFileName(files.FileName));
                        }
                        if (save != 0)
                        {
                            MAS_UID UId = new MAS_UID();
                            UId.FDate = TodaysDate;
                            UId.MacID = LoginMachinId;
                            UId.MacIP = LoginmachinIp;
                            UId.MAS_CHC_FID = individual1.MAS_CHC_FID;
                            UId.MAS_INDVSL_FID = individual.FID;
                            UId.U_Type = "Individual";
                            UId.U_ID = individual1.U_ID;
                            UId.U_Pass = individual1.U_Pass;
                            UId.MAS_LEADER_FID = Convert.ToInt32(LeaderFid);
                            UId.U_Status = true;
                            dbcontext.MAS_UID.Add(UId);
                            save = dbcontext.SaveChanges();
                        }
                    }

                    var GetChurchName = (from data in dbcontext.MAS_CHC where data.FID == individual1.MAS_CHC_FID && data.Status == true select data).FirstOrDefault();
                    SendMail mail = new SendMail();
                    var Title = "User ID and Password";
                    //var GetBody = "Dear <b>" + individual1.IND_Name + "</b>,<br><br>We send  your password and UserId </br></br><br>UserID : <b>" + individual1.U_ID + " <b></br><br></br><br>Passord : <b>" + individual1.U_Pass + "</b></br></br></b></br></br><br>Best regards.</br><br>Mumbai Church</br>";
                    var GetBody = "Dear <b>" + individual1.IND_Name + "</b>,<br><br>" +
                                         "Your default username and password are as follows:<br>" +
                                         "Username: <b>" + individual1.U_ID + "</b><br>" +
                                         "Password: <b>" + individual1.U_Pass + "</b><br><br>" +
                                         "To reset your username and password, click the link below:<br>" +
                                         "<a href='http://ekstasisministries.org/'>Reset Here</a><br><br>" +
                                         "Best regards,<br>" +
                                         GetChurchName.CHC_Name;
                    mail.SendMailToMember(individual1.IND_Email, GetBody, Title);
                }
                else
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }

                if (save != 0)
                {
                    TempData["Message"] = "Member Created Successfully";
                    TempData["Icon"] = "success";
                }
                else
                {
                    TempData["Message"] = "Member Not Created Successfully";
                    TempData["Icon"] = "error";
                }

                return RedirectToAction("IndividualCreation", "Leader", new { area = "Leader" });

            }
            catch (Exception ex)
            {

                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }
        }


        [HttpPost]
        public ActionResult CreateUserNamePass(int? FID, string UserName, string Password)
        {
            try
            {
                string LoginMachinIP = Dns.GetHostByName(LoginMachinId).AddressList[0].ToString();
                MAS_UID Uid = new MAS_UID();
                var Individual = dbcontext.MAS_INDVSL.Where(X => X.FID == FID).FirstOrDefault();
                //Uid.MAS_INDVSL_FID = Individual.FID;
                Uid.FDate = TodaysDate;
                Uid.MacID = LoginMachinId;
                Uid.MacIP = LoginMachinIP;
                Uid.MAS_INDVSL_FID = Individual.FID;
                Uid.U_Type = "Individual";
                Uid.U_ID = UserName;
                Uid.U_Pass = Password;
                Uid.U_Status = true;
                dbcontext.MAS_UID.Add(Uid);
                dbcontext.SaveChanges();
                TempData["SelfIndivisuals"] = "Username And Password Created Successfully";
                return RedirectToAction("SelfIndividualsList", "Leader", new { area = "Leader" });
            }
            catch (Exception Ex)
            {
                return View();
            }
        }
        #endregion Individuals

        #region GetCity
        public ActionResult GetCity(int? StateId)
        {
            try
            {
                if (Session["HeadLeader_U_Fid"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }

                var Data = (from data in dbcontext.MAS_POSTCOD
                            where data.State_Fid == StateId
                            select new BindDrop { Name = data.City })
                    .Distinct()
                    .ToList();
                return Json(Data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }
        }
        #endregion

        #region GetArea
        public ActionResult GetArea(int? StateId, string MasCityName)
        {
            try
            {
                if (Session["HeadLeader_U_Fid"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
                var Data = (from data in dbcontext.MAS_POSTCOD
                            where data.State_Fid == StateId && data.City == MasCityName
                            select new BindDrop { Name = data.Area })
                             .Distinct()
                            .ToList();

                //Get Church Name Start

                //var GetPostCode = dbcontext.MAS_POSTCOD
                //  .Where(X => X.City == MasCityName && X.State_Fid == StateId )
                //  .Select(X => X.PostalCode)
                //  .FirstOrDefault();

                var MasPostcodeFid = dbcontext.MAS_POSTCOD
                        .Where(X => X.City == MasCityName && X.State_Fid == StateId)
                        .Select(X => X.Fid)
                        .ToList();

                var GetChurch = (from data in dbcontext.MAS_CHC
                                 where MasPostcodeFid.Contains((int)data.Mas_Postcode_Fid) && data.Mas_State_Fid == StateId
                                 select new BindDrop { Id = data.FID, Name = data.CHC_Name }).ToList();               


                return Json(new { Data, GetChurch }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }
        }
        #endregion 

        #region NoticeCreation 
        public ActionResult NoticeCreate()
        {
            try
            {
                var LeaderDesignation = Session["LeaderDesignation"];
                if (Session["LeaderDesignation"] == null)
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

        //[HttpPost]
        //[ValidateInput(false)]
        //public ActionResult NoticeCreate(Mas_Notice Notice, string NoticeDescription, bool MealAtteande)
        //{
        //    try
        //    {
        //        var LeaderDesignation = Session["LeaderDesignation"];
        //        if (Session["LeaderDesignation"] == null)
        //        {
        //            return RedirectToAction("Login", "Home", new { area = "" });
        //        }


        //        Mas_Notice MasNotice = new Mas_Notice();
        //        var LoginmachinIp = Dns.GetHostByName(LoginMachinId).AddressList[0].ToString();
        //        //var LeaderFId = Session["LeaderFId"];
        //        //var curchFId = Session["LeaderCurchId"];
        //        var LeaderFId = 0;
        //        var LedercurchFId = 0;

        //        MasNotice.MacID = LoginMachinId;
        //        MasNotice.MacIP = LoginmachinIp;
        //        MasNotice.NoticeDate = Notice.NoticeDate;
        //        MasNotice.NoticeDateValid = Notice.NoticeDateValid;
        //        //MasNotice.NoticeDescription = Notice.NoticeDescription;
        //        MasNotice.NoticeDescription = NoticeDescription;
        //        MasNotice.NoticeSubject = Notice.NoticeSubject;
        //        MasNotice.FDate = DateTime.Now;
        //        MasNotice.Deactivate = false;
        //        MasNotice.NoticeViews = true;
        //        MasNotice.Meal_Attandence = MealAtteande;
        //        if (LeaderDesignation.ToString() == "Head Leader")
        //        {
        //            LeaderFId = Convert.ToInt32(Session["HeadLeader_U_Fid"]);
        //            LedercurchFId = Convert.ToInt32(Session["HeadLeaderCurchId"]);
        //            MasNotice.NoticeCreateBy = "IsHeadLeader";
        //        }
        //        else
        //        {
        //            LeaderFId = Convert.ToInt32(Session["LeaderFId"]);
        //            LedercurchFId = Convert.ToInt32(Session["LeaderCurchId"]);
        //            MasNotice.NoticeCreateBy = "IsLeader";
        //        }
        //        MasNotice.LeaderFId = Convert.ToInt32(LeaderFId);
        //        MasNotice.CurchId = Convert.ToInt32(LedercurchFId);

        //        dbcontext.Mas_Notice.Add(MasNotice);
        //        int SaveData = dbcontext.SaveChanges();


        //        if (SaveData != 0)
        //        {
        //            TempData["Message"] = "Notice Create Successfully !";
        //            TempData["Icon"] = "success";
        //        }
        //        else
        //        {
        //            TempData["Message"] = "Notice Not Create Successfully !";
        //            TempData["Icon"] = "error";
        //        }

        //        return View();
        //    }
        //    catch (Exception ex)
        //    {
        //        @Session["GetErrorMessage"] = ex.Message;
        //        return RedirectToAction("Error", "Home", new { area = "" });
        //    }
        //}
        #endregion

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> NoticeCreate(Mas_Notice Notice, string NoticeDescription, bool MealAtteande)
        {
            try
            {
                var LeaderDesignation = Session["LeaderDesignation"];
                if (Session["LeaderDesignation"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
                int SaveData = 0;

                Mas_Notice MasNotice = new Mas_Notice();
                var LoginmachinIp = Dns.GetHostByName(LoginMachinId).AddressList[0].ToString();
                //var LeaderFId = Session["LeaderFId"];
                //var curchFId = Session["LeaderCurchId"];
                var LeaderFId = 0;
                var LedercurchFId = 0;

                MasNotice.MacID = LoginMachinId;
                MasNotice.MacIP = LoginmachinIp;
                MasNotice.NoticeDate = Notice.NoticeDate;
                MasNotice.NoticeDateValid = Notice.NoticeDateValid;
                //MasNotice.NoticeDescription = Notice.NoticeDescription;
                MasNotice.NoticeDescription = NoticeDescription;
                MasNotice.NoticeSubject = Notice.NoticeSubject;
                MasNotice.FDate = DateTime.Now;
                MasNotice.Deactivate = false;
                MasNotice.NoticeViews = true;
                MasNotice.Meal_Attandence = MealAtteande;
                if (LeaderDesignation.ToString() == "Head Leader")
                {
                    LeaderFId = Convert.ToInt32(Session["HeadLeader_U_Fid"]);
                    LedercurchFId = Convert.ToInt32(Session["HeadLeaderCurchId"]);
                    MasNotice.NoticeCreateBy = "IsHeadLeader";
                }
                else
                {
                    LeaderFId = Convert.ToInt32(Session["LeaderFId"]);
                    LedercurchFId = Convert.ToInt32(Session["LeaderCurchId"]);
                    MasNotice.NoticeCreateBy = "IsLeader";
                }
                MasNotice.LeaderFId = Convert.ToInt32(LeaderFId);
                MasNotice.CurchId = Convert.ToInt32(LedercurchFId);

                dbcontext.Mas_Notice.Add(MasNotice);
                 SaveData = dbcontext.SaveChanges();

                int employeeId = LeaderFId;
                string requestUri = $"http://49.248.250.54:8081/api/sendnotice?employeeId={employeeId}";

                string token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJleHAiOjE3NTE2OTkzNzMsImlzcyI6Imh0dHA6Ly9DaHVyY2guY29tIiwiYXVkIjoiaHR0cDovL0NodXJjaC5jb20ifQ.BGZr5r5swcj8KT6dhc15mt14IWqUsKWNPdgfoWLqyVc";

                using (var client = new HttpClient())
                {

                    client.Timeout = TimeSpan.FromSeconds(30); // You can adjust the timeout as needed
                    
                    using (var request = new HttpRequestMessage(HttpMethod.Get, requestUri))
                    {
                        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                        // Optionally, add headers if necessary (for example, Content-Type if you're sending data)
                        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));                       
                        using (var response = await client.SendAsync(request))
                        {
                            if (response.IsSuccessStatusCode)
                            {
                                string responseContent = await response.Content.ReadAsStringAsync();

                                // Deserialize the response content to a dynamic object if the response contains JSON
                                var obj = JsonConvert.DeserializeObject<dynamic>(responseContent);

                                SaveData = 1;
                            }
                            else
                            {
                                // If the response failed, read the content for more details (optional)
                                string responseContent2 = await response.Content.ReadAsStringAsync();

                                SaveData = 1;
                            }
                        }
                    }
                }       
                
                if (SaveData != 0)
                {
                    TempData["Message"] = "Notice Create Successfully !";
                    TempData["Icon"] = "success";
                }
                else
                {
                    TempData["Message"] = "Notice Not Create Successfully !";
                    TempData["Icon"] = "error";
                }

                return View();
            }
            catch (Exception ex)
            {
                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }
        }

        #region EnquiryList 
        public ActionResult EnquiryList()
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
                    var GetEnquiry = dbcontext.sp_List_Enquiry(HeadLeaderCurchFId, LeaderDesignation.ToString(), null, LeaderStateFid.StateFid).ToList();
                    ViewBag.GetEnquiryList = GetEnquiry;
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

                    var GetEnquiry = dbcontext.sp_List_Enquiry(null,null,LeaderFid,null).ToList();
                    ViewBag.GetEnquiryList = GetEnquiry;
                    //if (GetEnquiry.Count == 0)
                    //{
                    //    TempData["Message"] = "No Pending Inquiries ";
                    //    TempData["Icon"] = "warning";
                    //    return RedirectToAction("Dashboard", "Leader", new { area = "Leader" });
                    //}
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

        #region SendEnquiryResponse 
        [HttpGet]
        public ActionResult SendResponse(int EnquiryFID, int MemberFID, string LeaderResponse, string MemberEmail, string LeaderResponseStatus)
        {
            try
            {
                int LeaderFid;
                var LeaderDesignation = Session["LeaderDesignation"];
                if (LeaderDesignation.ToString() == "Head Leader")
                {
                    if (Session["HeadLeader_U_Fid"] == null)
                    {
                        return RedirectToAction("Login", "Home", new { area = "" });
                    }
                    var HeadLeaderFid = Session["HeadLeader_U_Fid"];
                    LeaderFid = Convert.ToInt32(HeadLeaderFid);
                }
                else
                {
                    if (Session["LeaderFId"] == null)
                    {
                        return RedirectToAction("Login", "Home", new { area = "" });
                    }
                    var Leader_FId = Session["LeaderFId"];
                    LeaderFid = Convert.ToInt32(Leader_FId);
                }
                    int save = 0;
                var LeaderFId = Session["U_Fid"];
                Mas_Enquiry MasEnquiry = new Mas_Enquiry();
                var getdata = (from data in dbcontext.Mas_Enquiry where data.FId == EnquiryFID && data.MemberFId == MemberFID select data).FirstOrDefault();
                if (getdata != null)
                {
                    getdata.LeaderFId = LeaderFid;
                    getdata.ResponsedDate = DateTime.Now;
                    getdata.Respons = LeaderResponse;

                    if (LeaderResponseStatus == "WIP")
                    {
                        getdata.LeaderResponseStatus = true;
                        getdata.Status = false;
                    }
                    if (LeaderResponseStatus == "Close")
                    {
                        getdata.LeaderResponseStatus = false;
                        getdata.Status = false;
                    }

                    save = dbcontext.SaveChanges();
                }
                if (save != 0)
                {

                    var Message = "Your Response Send  SuccessFully !";
                    var Icon = "success";
                    return Json(new { Message, Icon }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var Message = "Your Response Not Send !";
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




    }
}