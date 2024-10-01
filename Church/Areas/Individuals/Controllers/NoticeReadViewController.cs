using Church.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Church.Areas.Individuals.Controllers
{
    public class NoticeReadViewController : Controller
    {
        CHC_NewEntities dbcontext = new CHC_NewEntities();
        DateTime TodaysDate = DateTime.Now;
        string LoginMachinId = Dns.GetHostName();
        // GET: Individuals/NoticeReadView
        public ActionResult NoticeReadView(int? NoticeFId, int? ChurchFid, int? DashboardView)
        {
            try
            {
                if (Session["U_Fid"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
                var IndvslFID = Session["U_Fid"];
                var IndvslName = Session["IndividualName"];
                var IndvslChurchFid = Session["IndvslUserCurchId"];
                int IndvslChurchFId = Convert.ToInt32(IndvslChurchFid);
                int IndvslFiD = Convert.ToInt32(IndvslFID);
                if (DashboardView != null)
                {
                    ViewBag.NoticeDashboardView = DashboardView;
                }
                //var GetData = (from data in dbcontext.Mas_Notice where data.FId == NoticeFId && data.CurchId == ChurchFid select data).FirstOrDefault();

                var checkAttendance = (from data in dbcontext.MealAttendances where data.MemberFid == IndvslFiD && data.NoticeFid == NoticeFId && data.Mas_ChurchFid == ChurchFid select data).FirstOrDefault();
                if(checkAttendance!=null)
                {
                    ViewBag.Attendance = "yes";
                    ViewBag.NoOfAdult = checkAttendance.NoOFAdult;
                    ViewBag.NoOfChild = checkAttendance.NoOfChild;
                }
                else
                {
                    ViewBag.Attendance = "no";
                }
             
                var GetData = dbcontext.sp_List_ReadNotice(NoticeFId, ChurchFid).FirstOrDefault();
                if (GetData == null)
                {
                    TempData["Message"] = "Something Wrong !";
                    TempData["Icon"] = "error";
                    return RedirectToAction("NoticeList", "Individuals", new { area = "Individuals" });
                }
                else
                {
                    ViewBag.NoticeData = GetData;
                }


                return View();
            }
            catch (Exception ex)
            {

                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }

        }

        [HttpPost]
        public ActionResult MemberMealAttendance(int? NoOfAdult, int? NoOfChild, int? NoticeFid)
        {
            try
            {
                if (Session["U_Fid"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
                var LoginMachinIp = Dns.GetHostByName(LoginMachinId).AddressList[0].ToString();
                int save = 0;

                var IndvslFID = Session["U_Fid"];
                var IndvslName = Session["IndividualName"];
                var IndvslChurchFid = Session["IndvslUserCurchId"];
                int IndvslChurchFId = Convert.ToInt32(IndvslChurchFid);
                int IndvslFiD = Convert.ToInt32(IndvslFID);


                var CheckAttendance = (from data in dbcontext.MealAttendances
                                       where data.MemberFid == IndvslFiD
                                        && data.NoticeFid == NoticeFid && data.Mas_ChurchFid == IndvslChurchFId
                                       select data).FirstOrDefault();

                if(CheckAttendance!=null)
                {
                    CheckAttendance.MemberFid = Convert.ToInt32(IndvslFID);
                    CheckAttendance.Mas_ChurchFid = Convert.ToInt32(IndvslChurchFid);
                    CheckAttendance.NoticeFid = NoticeFid;
                    CheckAttendance.NoOFAdult = NoOfAdult;
                    CheckAttendance.NoOfChild = NoOfChild;

                    save=dbcontext.SaveChanges();
                    if(save==0)
                    {
                        save = 1;
                    }
                }else
                {
                    MealAttendance Atten = new MealAttendance();
                    Atten.MacID = LoginMachinId;
                    Atten.MacIP = LoginMachinIp;
                    Atten.FDate = DateTime.Now;
                    Atten.MemberFid = Convert.ToInt32(IndvslFID);
                    Atten.Mas_ChurchFid = Convert.ToInt32(IndvslChurchFid);
                    Atten.NoticeFid = NoticeFid;
                    Atten.NoOFAdult = NoOfAdult;
                    Atten.NoOfChild = NoOfChild;
                    Atten.Deleted = false;

                    dbcontext.MealAttendances.Add(Atten);
                    save = dbcontext.SaveChanges();
                }              
                if (save != 0)
                {
                    Mas_ReadView ReadView = new Mas_ReadView();
                    var ReadViewData = (from data in dbcontext.Mas_ReadView where data.NoticeFId == NoticeFid && data.MemberFId == IndvslFiD select data).FirstOrDefault();

                    if (ReadViewData == null)
                    {
                        ReadView.MacID = LoginMachinId;
                        ReadView.MacIP = LoginMachinIp;
                        ReadView.FDate = DateTime.Now;
                        ReadView.NoticeFId = NoticeFid;
                        ReadView.MemberFId = Convert.ToInt32(IndvslFID);
                        ReadView.Counter = 1;

                        dbcontext.Mas_ReadView.Add(ReadView);
                        dbcontext.SaveChanges();
                    }
                }

                if (save != 0)
                {
                    TempData["Message1"] = "Attendance Submited";
                    TempData["Icon1"] = "success";
                    return RedirectToAction("NoticeReadView", "NoticeReadView", new { area = "Individuals", NoticeFId = NoticeFid, ChurchFid = IndvslChurchFId });
                }
                else
                {
                    TempData["Message"] = "Attendance Not Submited";
                    TempData["Icon"] = "error";
                    return RedirectToAction("NoticeReadView", "NoticeReadView", new { area = "Individuals", NoticeFId = NoticeFid, ChurchFid = IndvslChurchFId });
                }

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public ActionResult CancelMealAttendance(int? NoticeFid)
        {
            try
            {
                if (Session["U_Fid"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
                int save = 0;
                var IndvslFID = Session["U_Fid"];
                var IndvslName = Session["IndividualName"];
                var IndvslChurchFid = Session["IndvslUserCurchId"];
                int IndvslChurchFId = Convert.ToInt32(IndvslChurchFid);
                int IndvslFiD = Convert.ToInt32(IndvslFID);

                var CurrentDate = DateTime.Today;
                var CheckNoticeExpired = (from data in dbcontext.Mas_Notice where data.FId == NoticeFid && data.NoticeDateValid >= CurrentDate && data.Deactivate == false select data).FirstOrDefault();
                var CancelMealAttendance = (from data in dbcontext.MealAttendances where data.MemberFid == IndvslFiD && data.NoticeFid == NoticeFid && data.Deleted == false select data).FirstOrDefault();
                if(CheckNoticeExpired!=null)
                {
                    if (CancelMealAttendance != null)
                    {
                        CancelMealAttendance.Deleted = true;
                        save = dbcontext.SaveChanges();
                    }
                }else
                {
                    TempData["Message"] = "Notice has been expired";
                    TempData["Icon"] = "error";
                    return RedirectToAction("NoticeReadView", "NoticeReadView", new { area = "Individuals", NoticeFId = NoticeFid, ChurchFid = IndvslChurchFId });
                }               
                if (save != 0)
                {
                    TempData["Message"] = "Attendance Cancel";
                    TempData["Icon"] = "success";
                    return RedirectToAction("NoticeReadView", "NoticeReadView", new { area = "Individuals", NoticeFId = NoticeFid, ChurchFid = IndvslChurchFId });
                }
                else
                {
                    TempData["Message"] = "Attendance already Cancel";
                    TempData["Icon"] = "error";
                    return RedirectToAction("NoticeReadView", "NoticeReadView", new { area = "Individuals", NoticeFId = NoticeFid, ChurchFid = IndvslChurchFId });
                }
              
            }
            catch (Exception ex)
            {

                Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }
        }

        [HttpPost]
        public ActionResult UpdateAttendance(int? NoOfAdult, int? NoOfChild, int? NoticeFid)
        {
            try
            {
                if (Session["U_Fid"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
                
                int save = 0;
                var IndvslFID = Session["U_Fid"];
                var IndvslName = Session["IndividualName"];
                var IndvslChurchFid = Session["IndvslUserCurchId"];
                int IndvslChurchFId = Convert.ToInt32(IndvslChurchFid);
                int IndvslFiD = Convert.ToInt32(IndvslFID);

                var CurrentDate = DateTime.Today;
                var CheckNoticeExpired = (from data in dbcontext.Mas_Notice where data.FId == NoticeFid && data.NoticeDateValid >= CurrentDate && data.Deactivate == false select data).FirstOrDefault();

                var UpdateAttendance = (from data in dbcontext.MealAttendances where data.MemberFid == IndvslFiD && data.NoticeFid == NoticeFid
                                        && data.Mas_ChurchFid == IndvslChurchFId && data.Deleted == false select data).FirstOrDefault();
                if(CheckNoticeExpired!=null)
                {
                    if (UpdateAttendance != null)
                    {
                        UpdateAttendance.NoOFAdult = NoOfAdult;
                        UpdateAttendance.NoOfChild = NoOfChild;
                        save = dbcontext.SaveChanges();
                    }
                    else
                    {
                        TempData["Message1"] = "Already Cancel Attendance";
                        TempData["Icon1"] = "error";
                        return RedirectToAction("NoticeReadView", "NoticeReadView", new { area = "Individuals", NoticeFId = NoticeFid, ChurchFid = IndvslChurchFId });
                    }
                }else
                {
                    TempData["Message1"] = "Notice has been expired";
                    TempData["Icon1"] = "error";
                    return RedirectToAction("NoticeReadView", "NoticeReadView", new { area = "Individuals", NoticeFId = NoticeFid, ChurchFid = IndvslChurchFId });
                }
               
                if (save >= 0)
                {
                    TempData["Message1"] = "Attendance Updated";
                    TempData["Icon1"] = "success";
                    return RedirectToAction("NoticeReadView", "NoticeReadView", new { area = "Individuals", NoticeFId = NoticeFid, ChurchFid = IndvslChurchFId });
                }else
                {
                    TempData["Message1"] = "Attendance Updated";
                    TempData["Icon1"] = "success";
                    return RedirectToAction("NoticeReadView", "NoticeReadView", new { area = "Individuals", NoticeFId = NoticeFid, ChurchFid = IndvslChurchFId });
                }
            }
            catch (Exception ex)
            {

                Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }
        }


    }
}