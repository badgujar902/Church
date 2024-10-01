using Church.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Church.Areas.Admin.Controllers
{
    public class AdminCreateEventController : Controller
    {
        CHC_NewEntities dbcontext = new CHC_NewEntities();
        DateTime TodaysDate = DateTime.Now;
        string LoginMachinId = Dns.GetHostName();
        // GET: Admin/AdminCreateEvent
        public ActionResult AdminCreateEvent()
        {
            try
            {
                if (Session["Admin"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
            }
            catch (Exception ex)
            {
                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }
            return View();
        }


        #region SaveCreatedEvent   IEnumerable<HttpPostedFileBase> files
        [HttpPost]
        public ActionResult SaveCreatedEvent(string EventDescription, string VideoUrl, IEnumerable<HttpPostedFileBase> file)
        {
            try
            {
                //int fileCount = file?.Count() ?? 0;
                if (Session["Admin"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }

                //if (fileCount > 20)
                //{
                //    TempData["Message"] = "select Only 10 File";
                //    TempData["Icon"] = "error";
                //    return RedirectToAction("CreateEvent", "CreateEvent", new { area = "Leader" });
                //}
                var LoginmachinIp = Dns.GetHostByName(LoginMachinId).AddressList[0].ToString();

                //var ChurchFid = Session["LeaderCurchId"];
                //var LeaderFId = Session["LeaderFId"];
                int save = 0;

                if (file != null)
                {
                    Mas_Event Event = new Mas_Event();

                    Event.FDate = DateTime.Now;
                    Event.EventNote = EventDescription;
                    //Event.ChurchFId = Convert.ToInt32(ChurchFid);
                    //Event.LeaderFId = Convert.ToInt32(LeaderFId);
                    Event.Deleted = false;
                    Event.Edited = false;
                    Event.MacID = LoginMachinId;
                    Event.MacIP = LoginmachinIp;
                    Event.AdminEvent = "Admin";

                    dbcontext.Mas_Event.Add(Event);
                    save = dbcontext.SaveChanges();

                    if (save != 0)
                    {
                        int EvnetFid = dbcontext.Mas_Event.Max(e => (int?)e.FId) ?? 0;

                        Mas_EventPost eventpost = new Mas_EventPost();

                        string FileURl = "";
                        List<string> imageUrls = new List<string>();

                        foreach (var files in file)
                        {
                            string MoveLocation = @"C:\StealthChurch\Events\" + EvnetFid + "\\";
                            if (files != null && files.ContentLength > 0)
                            {
                                if (!Directory.Exists(MoveLocation))
                                {
                                    Directory.CreateDirectory(MoveLocation);
                                }
                                files.SaveAs(MoveLocation + Path.GetFileName((files.FileName)));
                                FileURl = Convert.ToString(MoveLocation + Path.GetFileName(files.FileName));
                                imageUrls.Add(FileURl);
                            }
                        }

                        eventpost.ImageURL = string.Join(",", imageUrls);
                        eventpost.FDate = DateTime.Now;
                        eventpost.EventFId = EvnetFid;
                        //eventpost.ChurchFId = Convert.ToInt32(ChurchFid);
                        //eventpost.LeaderFId = Convert.ToInt32(LeaderFId);
                        eventpost.MacID = LoginMachinId;
                        eventpost.MacIP = LoginmachinIp;
                        eventpost.Deactivate = false;
                        eventpost.AdminEvent = "Admin";



                        dbcontext.Mas_EventPost.Add(eventpost);
                        save = dbcontext.SaveChanges();
                    }
                    if (save != 0)
                    {
                        if (VideoUrl != null && VideoUrl != "")
                        {
                            int EvnetFid = dbcontext.Mas_Event.Max(e => (int?)e.FId) ?? 0;

                            Mas_EventVideo eventvideo = new Mas_EventVideo();
                            eventvideo.MacID = LoginMachinId;
                            eventvideo.MacIP = LoginmachinIp;
                            eventvideo.VideoUrl = VideoUrl;
                            //eventvideo.ChurchFId = Convert.ToInt32(ChurchFid);
                            eventvideo.EventFId = EvnetFid;
                            eventvideo.FDate = DateTime.Now;
                            //eventvideo.LeaderFId = Convert.ToInt32(LeaderFId);
                            eventvideo.Deactivate = false;
                            eventvideo.AdminEvent = "Admin";

                            dbcontext.Mas_EventVideo.Add(eventvideo);

                            save = dbcontext.SaveChanges();
                        }
                    }

                }
                else
                {
                    TempData["Message"] = "Please Drop Image";
                    TempData["Icon"] = "error";
                    return Json(new { Message = TempData["Message"], Icon = TempData["Icon"] }, JsonRequestBehavior.AllowGet);
                }

                if (save != 0)
                {
                    TempData["Message"] = "Event Created Successfully";
                    TempData["Icon"] = "success";
                    return Json(new { Message = TempData["Message"], Icon = TempData["Icon"] }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    TempData["Message"] = "Event Not Create Successfully";
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
        #endregion

        #region EventsList
        public ActionResult EventsList()
        {
            try
            {
        
                var Events = (from data in dbcontext.Mas_Event where  data.Deleted == false  orderby data.FId descending select data).ToList();
                ViewBag.EventsList = Events;
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