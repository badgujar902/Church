using Church.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Church.Areas.Leader.Controllers
{
    public class NoticeReadController : Controller
    {
        CHC_NewEntities dbcontext = new CHC_NewEntities();
        DateTime TodaysDate = DateTime.Now;
        string LoginMachinId = Dns.GetHostName();
        // GET: Leader/NoticeRead
        public ActionResult NoticeReadView(int? NoticeFId, int? ChurchFid, int? DashboardView)
        {
            try
            {
                if (Session["LeaderFId"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
                var LeaderFId = Session["LeaderFId"];
                var curchFId = Session["LeaderCurchId"];
                int LeaderCurch_Id = Convert.ToInt32(curchFId);
                int LeaderFid = Convert.ToInt32(LeaderFId);
                if (DashboardView != null)
                {
                    ViewBag.NoticeDashboardView = DashboardView;
                }
                //var GetData = (from data in dbcontext.Mas_Notice where data.FId == NoticeFId && data.CurchId == ChurchFid select data).FirstOrDefault();

                var GetData = dbcontext.sp_List_ReadNotice(NoticeFId, ChurchFid).FirstOrDefault();
                if (GetData == null)
                {
                    TempData["Message"] = "Something Wrong !";
                    TempData["Icon"] = "error";
                    return RedirectToAction("Dashboard", "Leader", new { area = "Leader" });
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


        public ActionResult NoticeReadByMember(int? NoticeFID, int? DashboardView)
        {
            try
            {
                if (Session["LeaderFId"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
                var LeaderFId = Session["LeaderFId"];
                var curchFId = Session["LeaderCurchId"];
                int LeaderCurch_Id = Convert.ToInt32(curchFId);
                int LeaderFid = Convert.ToInt32(LeaderFId);             

                var LoginmachinIp = Dns.GetHostByName(LoginMachinId).AddressList[0].ToString();
                Mas_ReadView ReadView = new Mas_ReadView();
                var ReadViewData = (from data in dbcontext.Mas_ReadView where data.NoticeFId == NoticeFID && data.MemberFId == LeaderFid select data).FirstOrDefault();               

                if (ReadViewData == null)
                {
                    ReadView.MacID = LoginMachinId;
                    ReadView.MacIP = LoginmachinIp;
                    ReadView.FDate = DateTime.Now;
                    ReadView.NoticeFId = NoticeFID;
                    ReadView.MemberFId = Convert.ToInt32(LeaderFid);
                    ReadView.Counter = 1;

                    dbcontext.Mas_ReadView.Add(ReadView);
                    dbcontext.SaveChanges();
                }
                if (DashboardView != null)
                {
                    return RedirectToAction("Dashboard", "Leader", new { area = "Leader" });
                }
                else
                {
                    return RedirectToAction("Dashboard", "Leader", new { area = "Leader" });
                }
                //else
                //{
                //    return RedirectToAction("NoticeList", "Leader", new { area = "Individuals" });
                //}


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
                
                var LoginMachinIp = Dns.GetHostByName(LoginMachinId).AddressList[0].ToString();
                int save = 0;

                if (Session["LeaderFId"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
                var LeaderFId = Session["LeaderFId"];
                var curchFId = Session["LeaderCurchId"];
                int LeaderCurch_Id = Convert.ToInt32(curchFId);
                int LeaderFid = Convert.ToInt32(LeaderFId);


                MealAttendance Atten = new MealAttendance();
                Atten.MacID = LoginMachinId;
                Atten.MacIP = LoginMachinIp;
                Atten.FDate = DateTime.Now;
                Atten.MemberFid = Convert.ToInt32(LeaderFid);
                Atten.Mas_ChurchFid = Convert.ToInt32(LeaderCurch_Id);
                Atten.NoticeFid = NoticeFid;
                Atten.NoOFAdult = NoOfAdult;
                Atten.NoOfChild = NoOfChild;

                dbcontext.MealAttendances.Add(Atten);
                save = dbcontext.SaveChanges();
                if (save != 0)
                {
                    Mas_ReadView ReadView = new Mas_ReadView();
                    var ReadViewData = (from data in dbcontext.Mas_ReadView where data.NoticeFId == NoticeFid && data.MemberFId == LeaderFid select data).FirstOrDefault();

                    if (ReadViewData == null)
                    {
                        ReadView.MacID = LoginMachinId;
                        ReadView.MacIP = LoginMachinIp;
                        ReadView.FDate = DateTime.Now;
                        ReadView.NoticeFId = NoticeFid;
                        ReadView.MemberFId = Convert.ToInt32(LeaderFId);
                        ReadView.Counter = 1;

                        dbcontext.Mas_ReadView.Add(ReadView);
                        dbcontext.SaveChanges();
                    }
                }

                if (save != 0)
                {
                    TempData["Message1"] = "Attendance Submited";
                    TempData["Icon1"] = "success";
                    return RedirectToAction("NoticeReadView", "NoticeRead", new { area = "Leader", NoticeFId = NoticeFid, ChurchFid = LeaderCurch_Id });
                    //return RedirectToAction("Dashboard", "Leader", new { area = "Leader" });
                }
                else
                {
                    TempData["Message"] = "Attendance Not Submited";
                    TempData["Icon"] = "error";
                    return RedirectToAction("NoticeReadView", "NoticeRead", new { area = "Leader", NoticeFId = NoticeFid, ChurchFid = LeaderCurch_Id });
                }

            }
            catch (Exception ex)
            {

                throw;
            }
        }



        //[HttpPost]
        //public ActionResult MemberMealAttendance(int? NoOfAdult, int? NoOfChild, int? NoticeFid)
        //{
        //    try
        //    {
        //        if (Session["LeaderFId"] == null)
        //        {
        //            return RedirectToAction("Login", "Home", new { area = "" });
        //        }
        //        var LeaderFId = Session["LeaderFId"];
        //        var curchFId = Session["LeaderCurchId"];
        //        int LeaderCurch_Id = Convert.ToInt32(curchFId);
        //        int LeaderFid = Convert.ToInt32(LeaderFId);

        //        var LoginMachinIp = Dns.GetHostByName(LoginMachinId).AddressList[0].ToString();
        //        int save = 0;



        //        MealAttendance Atten = new MealAttendance();
        //        Atten.MacID = LoginMachinId;
        //        Atten.MacIP = LoginMachinIp;
        //        Atten.FDate = DateTime.Now;
        //        Atten.MemberFid = Convert.ToInt32(LeaderFid);
        //        Atten.Mas_ChurchFid = Convert.ToInt32(LeaderCurch_Id);
        //        Atten.NoticeFid = NoticeFid;
        //        Atten.NoOFAdult = NoOfAdult;
        //        Atten.NoOfChild = NoOfChild;

        //        dbcontext.MealAttendances.Add(Atten);
        //        save = dbcontext.SaveChanges();

        //        if (save != 0)
        //        {
        //            TempData["Message"] = "Attendance Submited";
        //            TempData["Icon"] = "success";
        //            return RedirectToAction("NoticeReadView", "NoticeRead", new { area = "Leader", NoticeFId = NoticeFid, ChurchFid = LeaderCurch_Id });
        //        }
        //        else
        //        {
        //            TempData["Message"] = "Attendance Not Submited";
        //            TempData["Icon"] = "error";
        //            return RedirectToAction("NoticeReadView", "NoticeRead", new { area = "Leader", NoticeFId = NoticeFid, ChurchFid = LeaderCurch_Id });
        //        }

        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }
        //}
    }
}