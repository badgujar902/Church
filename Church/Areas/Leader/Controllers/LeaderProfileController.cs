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
    public class LeaderProfileController : Controller
    {
        CHC_NewEntities dbcontext = new CHC_NewEntities();
        DateTime TodaysDate = DateTime.Now;
        string LoginMachinId = Dns.GetHostName();
        // GET: Leader/LeaderProfile
        public ActionResult LeaderProfile()
        {
            try
            {
                var LeaderDesignation = Session["LeaderDesignation"];

                //var PrayerRequest = dbcontext.MAS_PrayerReq.Where(x => x.Req_Status == false && x.MAS_CHC_FID == LeaderCurch_Id && x.Req_ID != null).ToList();
                //TempData["PrayerRequestCount"] = PrayerRequest.Count();

                if (LeaderDesignation.ToString() == "Head Leader")
                {
                    if (Session["HeadLeader_U_Fid"] == null)
                    {
                        return RedirectToAction("Login", "Home", new { area = "" });
                    }
                    var HeadLeaderCurchId = Session["HeadLeaderCurchId"];
                    int HeadLeaderCurchFId = Convert.ToInt32(HeadLeaderCurchId);
                    var HeadLeaderFid = Session["HeadLeader_U_Fid"];
                    int HeadLeaderFID = Convert.ToInt32(HeadLeaderFid);

                    var GetLeaderProfileData = dbcontext.sp_List_GetLeaderDetail(HeadLeaderFID, HeadLeaderCurchFId).FirstOrDefault();
                    ViewBag.MemberProfileData = GetLeaderProfileData;

                    if (GetLeaderProfileData != null)
                    {
                        var BasUrl = (from data in dbcontext.DocumentBaseUrls select data).FirstOrDefault();
                        if (BasUrl != null)
                        {
                            string ImageUrl = GetLeaderProfileData.ImageUrl;
                            if (ImageUrl != null)
                            {
                                string fileName = Path.GetFileName(ImageUrl);
                                string baseUrl = BasUrl.BaseUrl;
                                string relativePath = ImageUrl.Replace("C:\\", "").Replace("\\", "/");
                                string finalImageUrl = baseUrl + relativePath;
                                ViewBag.LeaderProfileUrl = finalImageUrl;

                                Session["LeaderProfile"] = finalImageUrl;
                            }
                            else
                            {
                                ViewBag.LeaderProfileUrl = "";
                                Session["LeaderProfile"] = "";
                            }


                            //ViewBag.MemberProfileUrl = finalImageUrl;

                        }
                        else
                        {
                            ViewBag.LeaderProfileUrl = "";
                            Session["LeaderProfile"] = "";
                        }

                    }

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

                    var GetLeaderProfileData = dbcontext.sp_List_GetLeaderDetail(LeaderFid, LeaderCurch_Id).FirstOrDefault();
                    ViewBag.MemberProfileData = GetLeaderProfileData;


                    //if (GetLeaderProfileData != null)
                    //{
                    //    string ImageUrl = GetLeaderProfileData.ImageUrl;
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
                    //                    ViewBag.LeaderProfileUrl = FinalPath;
                    //                }
                    //            }
                    //        }
                    //        catch (Exception ex)
                    //        {
                    //            ViewBag.LeaderProfileUrl = "";
                    //            return View();
                    //        }
                    //    }
                    //}
                    if (GetLeaderProfileData != null)
                    {
                        var BasUrl = (from data in dbcontext.DocumentBaseUrls select data).FirstOrDefault();
                        if (BasUrl != null)
                        {
                            string ImageUrl = GetLeaderProfileData.ImageUrl;
                            if (ImageUrl != null)
                            {
                                string fileName = Path.GetFileName(ImageUrl);
                                string baseUrl = BasUrl.BaseUrl;
                                string relativePath = ImageUrl.Replace("C:\\", "").Replace("\\", "/");
                                string finalImageUrl = baseUrl + relativePath;
                                ViewBag.LeaderProfileUrl = finalImageUrl;
                                Session["LeaderProfile"] = finalImageUrl;
                            }
                            else
                            {
                                ViewBag.LeaderProfileUrl = "";
                                Session["LeaderProfile"] = "";
                            }


                            //ViewBag.MemberProfileUrl = finalImageUrl;

                        }
                        else
                        {
                            ViewBag.LeaderProfileUrl = "";
                            Session["LeaderProfile"] = "";
                        }

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

        #region UpdateProfile
        [HttpPost]
        public ActionResult UpdateProfile(HttpPostedFileBase file)
        {
            try
            {
                int update = 0;

                var LeaderDesignation = Session["LeaderDesignation"];
                HttpPostedFile files = System.Web.HttpContext.Current.Request.Files["file"];
                if (LeaderDesignation.ToString() == "Head Leader")
                {
                    if (Session["HeadLeader_U_Fid"] == null)
                    {
                        return RedirectToAction("Login", "Home", new { area = "" });
                    }
                    var HeadLeaderCurchId = Session["HeadLeaderCurchId"];
                    int HeadLeaderCurchFId = Convert.ToInt32(HeadLeaderCurchId);
                    var HeadLeaderFid = Session["HeadLeader_U_Fid"];
                    int HeadLeaderFID = Convert.ToInt32(HeadLeaderFid);
                    if (file != null)
                    {
                        var ProfileData = (from data in dbcontext.MAS_LEADER where data.FID == HeadLeaderFID && data.MAS_CHC_FID == HeadLeaderCurchFId && data.Status == true select data).FirstOrDefault();
                        if (ProfileData != null)
                        {
                            var filelctn = ProfileData.Lead_Mob;
                            string MoveLocation = @"C:\StealthChurch\LeaderCreatetionData\" + filelctn + "\\";
                            if (files.ContentLength > 0)
                            {
                                if (!Directory.Exists(MoveLocation))
                                {
                                    Directory.CreateDirectory(MoveLocation);
                                }
                                files.SaveAs(MoveLocation + Path.GetFileName((files.FileName)));
                                ProfileData.Image = Convert.ToString(MoveLocation + Path.GetFileName(files.FileName));

                            }
                            update = dbcontext.SaveChanges();
                        }
                    }
                    if (update >= 0)
                    {
                        TempData["Message"] = "Profile Picture Updated Successfuly";
                        TempData["Icon"] = "success";
                        return Json(new { Message = TempData["Message"], Icon = TempData["Icon"] }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        TempData["Message"] = "Profile Picture Not Updated Successfuly";
                        TempData["Icon"] = "error";
                        return Json(new { Message = TempData["Message"], Icon = TempData["Icon"] }, JsonRequestBehavior.AllowGet);
                    }
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
                    if (file != null)
                    {
                        var ProfileData = (from data in dbcontext.MAS_LEADER where data.FID == LeaderFid && data.MAS_CHC_FID == LeaderCurch_Id && data.Status == true select data).FirstOrDefault();
                        if (ProfileData != null)
                        {
                            var filelctn = ProfileData.Lead_Mob;
                            string MoveLocation = @"C:\StealthChurch\LeaderCreatetionData\" + filelctn + "\\";
                            if (files.ContentLength > 0)
                            {
                                if (!Directory.Exists(MoveLocation))
                                {
                                    Directory.CreateDirectory(MoveLocation);
                                }
                                files.SaveAs(MoveLocation + Path.GetFileName((files.FileName)));
                                ProfileData.Image = Convert.ToString(MoveLocation + Path.GetFileName(files.FileName));

                            }
                            update = dbcontext.SaveChanges();
                        }
                    }
                    if (update >= 0)
                    {
                        TempData["Message"] = "Profile Picture Updated Successfuly";
                        TempData["Icon"] = "success";
                        return Json(new { Message = TempData["Message"], Icon = TempData["Icon"] }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        TempData["Message"] = "Profile Picture Not Updated Successfuly";
                        TempData["Icon"] = "error";
                        return Json(new { Message = TempData["Message"], Icon = TempData["Icon"] }, JsonRequestBehavior.AllowGet);
                    }
                }





            }
            catch (Exception ex)
            {
                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }
        }
        #endregion



        #region UpdateLeaderDetails
        public ActionResult UpdateLeaderDetails(string pk, string value, int LeaderFId)
        {
            try
            {
                var UpdateData = (from data in dbcontext.MAS_LEADER where data.Status == true && data.FID == LeaderFId select data).FirstOrDefault();

                if (pk == "LeaderName")
                {
                    UpdateData.Lead_Name = value;
                }
                else if (pk == "contact")
                {
                    UpdateData.Lead_Mob = value;
                }
                else if (pk == "Email")
                {
                    UpdateData.Lead_EmailID = value;
                }
                else if (pk == "Gender")
                {
                    UpdateData.Gender = value;
                }
                else if (pk == "DateOfBirth")
                {
                    DateTime date = Convert.ToDateTime(value);
                    UpdateData.Lead_DOB = date;
                }
                else if (pk == "Address")
                {
                    UpdateData.Lead_Add = value;
                }
                int i = dbcontext.SaveChanges();
                if (i >= 0)
                {
                    return RedirectToAction("LeaderProfile", "LeaderProfile", new { area = "Leader" });
                }
                else
                {
                    return RedirectToAction("LeaderProfile", "LeaderProfile", new { area = "Leader" });
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