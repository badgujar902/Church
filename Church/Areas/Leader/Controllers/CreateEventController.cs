using Church.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Church.Areas.Leader.Controllers
{
    public class CreateEventController : Controller
    {
        CHC_NewEntities dbcontext = new CHC_NewEntities();
        DateTime TodaysDate = DateTime.Now;
        string LoginMachinId = Dns.GetHostName();
        // GET: Leader/CreateEvent
        public ActionResult CreateEvent()
        {
            try
            {
                if (Session["HeadLeader_U_Fid"] == null)
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


        #region SaveCreatedEvent   IEnumerable<HttpPostedFileBase> files
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SaveCreatedEvent(string EventDescription, string VideoUrl, IEnumerable<HttpPostedFileBase> file)
        {
            try
            {
                //int fileCount = file?.Count() ?? 0;
                if (Session["HeadLeader_U_Fid"] == null)
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
                var HeadLeaderFId = Session["HeadLeader_U_Fid"];
                var HeadLeadercurchFId = Session["HeadLeaderCurchId"];

                int save = 0;

                //if (file != null)
                //{
                Mas_Event Event = new Mas_Event();

                Event.FDate = DateTime.Now;
                Event.EventNote = EventDescription;
                Event.ChurchFId = Convert.ToInt32(HeadLeadercurchFId);
                Event.LeaderFId = Convert.ToInt32(HeadLeaderFId);
                Event.Deleted = false;
                Event.Edited = false;
                Event.MacID = LoginMachinId;
                Event.MacIP = LoginmachinIp;

                dbcontext.Mas_Event.Add(Event);
                save = dbcontext.SaveChanges();

                if (save != 0)
                {
                    int EvnetFid = dbcontext.Mas_Event.Max(e => (int?)e.FId) ?? 0;

                    Mas_EventPost eventpost = new Mas_EventPost();

                    string FileURl = "";
                    List<string> imageUrls = new List<string>();
                    if (file != null)
                    {
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

                    }

                    eventpost.ImageURL = string.Join(",", imageUrls);
                    eventpost.FDate = DateTime.Now;
                    eventpost.EventFId = EvnetFid;
                    eventpost.ChurchFId = Convert.ToInt32(HeadLeadercurchFId);
                    eventpost.LeaderFId = Convert.ToInt32(HeadLeaderFId);
                    eventpost.MacID = LoginMachinId;
                    eventpost.MacIP = LoginmachinIp;
                    eventpost.Deactivate = false;



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
                        eventvideo.ChurchFId = Convert.ToInt32(HeadLeadercurchFId);
                        eventvideo.EventFId = EvnetFid;
                        eventvideo.FDate = DateTime.Now;
                        eventvideo.LeaderFId = Convert.ToInt32(HeadLeadercurchFId);
                        eventvideo.Deactivate = false;

                        dbcontext.Mas_EventVideo.Add(eventvideo);

                        save = dbcontext.SaveChanges();
                    }
                }

                //}
                //else
                //{
                //    TempData["Message"] = "Please Drop Image";
                //    TempData["Icon"] = "error";
                //    return Json(new { Message = TempData["Message"], Icon = TempData["Icon"] }, JsonRequestBehavior.AllowGet);
                //}

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
                if (Session["HeadLeader_U_Fid"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
                var HeadLeaderFId = Session["HeadLeader_U_Fid"];
                var HeadLeadercurchFId = Session["HeadLeaderCurchId"];
                int HeadLeaderCurchFId = Convert.ToInt32(HeadLeadercurchFId);
                int HeadLeader_FId = Convert.ToInt32(HeadLeaderFId);

                var Events = (from data in dbcontext.Mas_Event where data.ChurchFId == HeadLeaderCurchFId && data.Deleted == false && data.LeaderFId == HeadLeader_FId orderby data.FId descending select data).ToList();
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
        public ActionResult DeleteEvent(int? FId, int? ChurchFId)
        {
            try
            {
                if (Session["HeadLeader_U_Fid"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
                var MemberFid = Session["U_Fid"];
                int MemberFId = Convert.ToInt32(MemberFid);
                var DeleteEvent = (from data in dbcontext.Mas_Event where data.FId == FId && data.ChurchFId == ChurchFId select data).FirstOrDefault();

                DeleteEvent.Deleted = true;
                int i = dbcontext.SaveChanges();
                if (i >= 0)
                {
                    var DeleteEventPost = (from data in dbcontext.Mas_EventPost where data.EventFId == FId && data.ChurchFId == ChurchFId select data).FirstOrDefault();
                    DeleteEventPost.Deactivate = true;
                    int Delete = dbcontext.SaveChanges();   
                    if (Delete >= 1)
                    {
                        TempData["Message"] = "Event Deleted Successfully";
                        TempData["Icon"] = "success";
                        return RedirectToAction("EventsList", "CreateEvent", new { area = "Leader" });
                    }
                    else
                    {
                        TempData["Message"] = "Event Not Deleted Successfully";
                        TempData["Icon"] = "success";
                        return RedirectToAction("EventsList", "CreateEvent", new { area = "Leader" });
                    }
                }
                else
                {
                    TempData["Message"] = "Event Not Deleted Successfully";
                    TempData["Icon"] = "error";
                    return RedirectToAction("EventsList", "CreateEvent", new { area = "Leader" });
                }

            }
            catch (Exception ex)
            {
                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }
        }

        #region Event Update
        public ActionResult UpdateEvent(int? FID, int? ChurchFId)
        {
            try
            {


                if (Session["HeadLeader_U_Fid"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
                var EventNote = (from data in dbcontext.Mas_Event where data.FId == FID && data.ChurchFId == ChurchFId select data).FirstOrDefault();
                ViewBag.EventNote = EventNote;
                ViewBag.EventFid = EventNote.FId;
                ViewBag.ChurchFid = EventNote.ChurchFId;

                var EventPostImages = (from data in dbcontext.Mas_EventPost where data.EventFId == FID && data.ChurchFId == ChurchFId select data).FirstOrDefault();

                var EventVideoUrl = (from data in dbcontext.Mas_EventVideo where data.EventFId == FID && data.ChurchFId == ChurchFId && data.Deactivate == false select data).FirstOrDefault();

                if (EventVideoUrl != null)
                {
                    ViewBag.EventVideoUrl = EventVideoUrl.VideoUrl;
                }
                else
                {
                    ViewBag.EventVideoUrl = "";
                }

                ViewBag.EventDesc = EventNote.EventNote;




                ViewBag.EventPostImages = EventPostImages;

                return View();
            }
            catch (Exception ex)
            {
                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }
        }

        #endregion

        [HttpPost]       
        [ValidateInput(false)]
        public ActionResult SaveUpdateEvent(int? EventFid, int? ChurchId, string VideoUrl, string EventDescription, IEnumerable<HttpPostedFileBase> file, FormCollection form)
        {
            try
            {
                if (Session["HeadLeader_U_Fid"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }




                int update = 0;
                var UpdateEvent = (from data in dbcontext.Mas_Event where data.FId == EventFid && data.ChurchFId == ChurchId && data.Deleted == false select data).FirstOrDefault();
                var UpdateEventPost = (from data in dbcontext.Mas_EventPost where data.EventFId == EventFid && data.ChurchFId == ChurchId && data.Deactivate == false select data).FirstOrDefault();
                var UpdateEventVideoUrl = (from data in dbcontext.Mas_EventVideo where data.EventFId == EventFid && data.ChurchFId == ChurchId && data.Deactivate == false select data).FirstOrDefault();

                UpdateEvent.EventNote = EventDescription;
                update = dbcontext.SaveChanges();
                if (update >= 0)
                {

                    if (file != null)
                    {
                        int TotalImage = 0;
                        var FileCount = file.Count();
                        string FileURl = "";
                        List<string> imageUrls = new List<string>();
                        int url = 0;
                        while (form["ImageUrl" + url] != null)
                        {
                            imageUrls.Add(form["ImageUrl" + url]);
                            url++;
                        }

                        TotalImage = Convert.ToInt32(FileCount) + url;
                        if (TotalImage > 5)
                        {
                            TempData["Message"] = "Upload only 5 images aready upload image is " + url;
                            TempData["Icon"] = "error";
                            return Json(new { Message = TempData["Message"], Icon = TempData["Icon"] }, JsonRequestBehavior.AllowGet);
                        }



                        foreach (var files in file)
                        {
                            string MoveLocation = @"C:\StealthChurch\Events\\" + EventFid + "\\" + "\\" + files.FileName + "\\";
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
                        UpdateEventPost.ImageURL = string.Join(",", imageUrls);
                        update = dbcontext.SaveChanges();
                    }
                    else
                    {

                        List<string> imageUrls = new List<string>();
                        int url = 0;
                        while (form["ImageUrl" + url] != null)
                        {
                            imageUrls.Add(form["ImageUrl" + url]);
                            url++;
                        }
                        UpdateEventPost.ImageURL = string.Join(",", imageUrls);
                        update = dbcontext.SaveChanges();
                    }


                    if (update >= 0)
                    {
                        if (VideoUrl != "")
                        {
                            UpdateEventVideoUrl.VideoUrl = VideoUrl;
                            update = dbcontext.SaveChanges();
                        }


                        if (update >= 0)
                        {
                            TempData["Message"] = "Event Updated Successfuly";
                            TempData["Icon"] = "success";
                            return Json(new { Message = TempData["Message"], Icon = TempData["Icon"] }, JsonRequestBehavior.AllowGet);


                        }
                        else
                        {

                            TempData["Message"] = "Event Not Updated Successfuly";
                            TempData["Icon"] = "error";
                            return Json(new { Message = TempData["Message"], Icon = TempData["Icon"] }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        TempData["Message"] = "Event Not Updated Successfuly";
                        TempData["Icon"] = "error";
                        return Json(new { Message = TempData["Message"], Icon = TempData["Icon"] }, JsonRequestBehavior.AllowGet);
                    }

                }
                else
                {
                    TempData["Message"] = "Event Not Updated Successfuly";
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
    }
}