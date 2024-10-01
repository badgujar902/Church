using Church.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Church.Areas.Leader.Controllers
{
    public class NoticeUpdateController : Controller
    {
        CHC_NewEntities dbcontext = new CHC_NewEntities();
        DateTime TodaysDate = DateTime.Now;
        string LoginMachinId = Dns.GetHostName();
        // GET: Leader/NoticeUpdate
        public ActionResult NoticeList()
        {
            try
            {
                var LeaderDesignation = Session["LeaderDesignation"];
                if (Session["LeaderDesignation"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
                if(LeaderDesignation.ToString()== "Head Leader")
                {
                    var HeadLeaderFId = Session["HeadLeader_U_Fid"];
                    var HeadLeadercurchFId = Session["HeadLeaderCurchId"];
                    int HeadLeaderCurchFId = Convert.ToInt32(HeadLeadercurchFId);
                    int HeadLeader_FId = Convert.ToInt32(HeadLeaderFId);
                    //var NoticeData = (from data in dbcontext.Mas_Notice where data.Deactivate==false orderby data.NoticeDate descending select data).ToList();
                    var NoticeData = (from data in dbcontext.Mas_Notice where data.Deactivate == false && data.CurchId == HeadLeaderCurchFId && data.LeaderFId == HeadLeader_FId && data.NoticeDateValid >= DateTime.Today orderby data.NoticeDate descending select data).ToList();
                    ViewBag.NoticeList = NoticeData;
                }else
                {
                    var LeaderFId = Session["LeaderFId"];
                    var curchFId = Session["LeaderCurchId"];
                    int LeaderCurch_Id = Convert.ToInt32(curchFId);
                    int LeaderFid = Convert.ToInt32(LeaderFId);
                    var NoticeData = (from data in dbcontext.Mas_Notice where data.Deactivate == false && data.CurchId == LeaderCurch_Id && data.LeaderFId == LeaderFid && data.NoticeDateValid >= DateTime.Today orderby data.NoticeDate descending select data).ToList();
                    ViewBag.NoticeList = NoticeData;
                }
               

                return View();
            }
            catch (Exception ex)
            {
                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }
        }
        public ActionResult NoticeUpdate(int? FId,int? CurchId)
        {
            try
            {
                var LeaderDesignation = Session["LeaderDesignation"];
                if (Session["LeaderDesignation"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
                Mas_Notice notice = new Mas_Notice();
                notice = (from data in dbcontext.Mas_Notice where data.FId == FId && data.CurchId == CurchId && data.Deactivate==false select data).FirstOrDefault();

                if(notice.NoticeDateValid < DateTime.Today)
                {
                    TempData["Message"] = "Notice has been expired ! You can't update";
                    TempData["Icon"] = "error";
                    return RedirectToAction("NoticeList", "NoticeUpdate", new { area = "Leader" });
                }
                return View(notice);
            }
            catch (Exception ex)
            {
                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }
        
        }


        #region SaveUpdateNotice
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SaveUpdateNotice(Mas_Notice notice, string NoticeDescription)
        {
            try
            {
                if (Session["LeaderDesignation"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }

                var updateNotice = (from data in dbcontext.Mas_Notice where data.FId == notice.FId && data.CurchId == notice.CurchId && data.Deactivate == false select data).FirstOrDefault();

                updateNotice.NoticeSubject = notice.NoticeSubject;
                updateNotice.NoticeDescription = NoticeDescription;
                updateNotice.NoticeDate = notice.NoticeDate;
                updateNotice.NoticeDateValid = notice.NoticeDateValid;

                int save = dbcontext.SaveChanges();
                if (save >= 0)
                {
                    TempData["Message"] = "Notice Updated successfully";
                    TempData["Icon"] = "success";
                    return RedirectToAction("NoticeList", "NoticeUpdate", new { area = "Leader" });
                }
                else
                {
                    TempData["Message"] = "Notice Not Updated successfully";
                    TempData["Icon"] = "error";
                    return RedirectToAction("NoticeList", "NoticeUpdate", new { area = "Leader" });
                }

            }
            catch (Exception ex)
            {
                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }
        }
        #endregion

        #region DeleteNotice
        public ActionResult DeleteNotice(int? FId, int? CurchId)
        {
            try
            {
                if (Session["LeaderDesignation"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
                var DeleteNotice = (from data in dbcontext.Mas_Notice where data.FId == FId && data.CurchId == CurchId && data.Deactivate == false select data).FirstOrDefault();
                DeleteNotice.Deactivate = true;

                int save = dbcontext.SaveChanges();
                if (save != 0)
                {
                    TempData["Message"] = "Notice Deleted successfully";
                    TempData["Icon"] = "success";
                    return RedirectToAction("NoticeList", "NoticeUpdate", new { area = "Leader" });
                }
                else
                {
                    TempData["Message"] = "Notice Not Deleted successfully";
                    TempData["Icon"] = "error";
                    return RedirectToAction("NoticeList", "NoticeUpdate", new { area = "Leader" });
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