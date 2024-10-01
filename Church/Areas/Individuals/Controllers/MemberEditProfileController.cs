using Church.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Church.Areas.Individuals.Controllers
{
    public class MemberEditProfileController : Controller
    {
        CHC_NewEntities dbcontext = new CHC_NewEntities();
        DateTime TodaysDate = DateTime.Now;
        string LoginMachinId = Dns.GetHostName();
        // GET: Individuals/MemberEditProfile
        public ActionResult MemberEditProfile(int? id)
        {
            try
            {
                if (Session["U_Fid"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }

                MAS_INDVSL MemberDetails = new MAS_INDVSL();

                MemberDetails = (from data in dbcontext.MAS_INDVSL where data.FID == id select data).FirstOrDefault();

                string imageNameWithExtension = Path.GetFileName(MemberDetails.IND_Image);
                ViewBag.file = imageNameWithExtension;
                return View(MemberDetails);
            }
            catch (Exception ex)
            {
                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }      


        }
        [HttpPost]
        public ActionResult UpdateMemberEditProfile(MAS_INDVSL MemberDetails, HttpPostedFileBase file)
        {
            try
            {
                if (Session["U_Fid"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
                HttpPostedFile files = System.Web.HttpContext.Current.Request.Files["file"];

            

                int update = 0;
                int profileExist = 0;              

                if (MemberDetails!=null)
                {
                    var updatData = (from data in dbcontext.MAS_INDVSL where data.FID == MemberDetails.FID select data).FirstOrDefault();
                    updatData.IND_Name = MemberDetails.IND_Name;
                    updatData.Gender = MemberDetails.Gender;
                    updatData.IND_Mob = MemberDetails.IND_Mob;
                    updatData.IND_DOB = MemberDetails.IND_DOB;
                    updatData.IND_Email = MemberDetails.IND_Email;
                    updatData.IND_Address = MemberDetails.IND_Address;
                    updatData.AdharNo = MemberDetails.AdharNo;

                    var filelctn = updatData.IND_Mob;
                    string MoveLocation = @"C:\StealthChurch\RegistrationData\" + filelctn + "\\";
                    if(files!=null)
                    {
                        if (files.ContentLength > 0)
                        {
                            if (!Directory.Exists(MoveLocation))
                            {
                                Directory.CreateDirectory(MoveLocation);
                                profileExist = 1;
                            }
                            if (profileExist == 1)
                            {
                                files.SaveAs(MoveLocation + Path.GetFileName((files.FileName)));
                                updatData.IND_Image = Convert.ToString(MoveLocation + Path.GetFileName(files.FileName));
                            }

                        }
                    }                  

                    update = dbcontext.SaveChanges();

                }
                if(update>=0)
                {
                    TempData["Message"] = "Profile Updated Successfully";
                    TempData["Icon"] = "success";
                    return RedirectToAction("UserProfile", "Individuals", new { area = "Individuals" });
                }
                else
                {
                    TempData["Message"] = "Profile Not Updated Successfully";
                    TempData["Icon"] = "error";
                    return RedirectToAction("UserProfile", "Individuals", new { area = "Individuals" });
                }

               
            }
            catch (Exception ex)
            {

                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }
        }

        [HttpPost]
        public ActionResult UpdateProfile(HttpPostedFileBase file)
        {
            try
            {
                if (Session["U_Fid"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
                int update = 0;
                HttpPostedFile files = System.Web.HttpContext.Current.Request.Files["file"];
                var MemberFid = Session["U_Fid"];
                int MemberFID = Convert.ToInt32(MemberFid);
                var ChurchFid = Session["IndvslUserCurchId"];
                int ChurchFId = Convert.ToInt32(ChurchFid);
                if (file != null)
                {
                    var ProfileData = (from data in dbcontext.MAS_INDVSL where data.FID == MemberFID && data.MAS_CHC_FID== ChurchFId && data.Deactivate == false select data).FirstOrDefault();
                    if(ProfileData!=null)
                    {
                        var filelctn = ProfileData.IND_Mob;
                        string MoveLocation = @"C:\StealthChurch\RegistrationData\" + filelctn + "\\";
                        if (files.ContentLength > 0)
                        {
                            if (!Directory.Exists(MoveLocation))
                            {
                                Directory.CreateDirectory(MoveLocation);
                            }
                            files.SaveAs(MoveLocation + Path.GetFileName((files.FileName)));
                            ProfileData.IND_Image = Convert.ToString(MoveLocation + Path.GetFileName(files.FileName));
                        }
                        update = dbcontext.SaveChanges();
                    }               

                  
                }
           

                if(update>=0)
                {
                    TempData["Message"] = "Profile Picture Updated Successfuly";
                    TempData["Icon"] = "success";
                    return Json(new { Message = TempData["Message"], Icon = TempData["Icon"] }, JsonRequestBehavior.AllowGet);
                }else
                {
                    TempData["Message"] = "Profile Picture Not Updated Successfuly";
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

        #region UpdateMemberDetails
        public ActionResult UpdateMemberDetails(string pk, string value, int MemberFId)
        {
            try
            {
                //var UpdateData = (from data in dbcontext.MAS_LEADER where data.Status == true && data.FID == MemberFId select data).FirstOrDefault();
                var UpdateData = (from data in dbcontext.MAS_INDVSL where data.FID == MemberFId select data).FirstOrDefault();

                if (pk == "MemberName")
                {
                    UpdateData.IND_Name = value;
                }
                else if (pk == "contact")
                {
                    UpdateData.IND_Mob = value;
                }
                else if (pk == "Email")
                {
                    UpdateData.IND_Email = value;
                }
                else if (pk == "Gender")
                {
                    UpdateData.Gender = value;
                }
                else if (pk == "DateOfBirth")
                {
                    DateTime date = Convert.ToDateTime(value);
                    UpdateData.IND_DOB = date;
                }
                else if (pk == "Address")
                {
                    UpdateData.IND_Address = value;
                }
                int i = dbcontext.SaveChanges();
                if (i >= 0)
                {
                    return RedirectToAction("UserProfile", "Individuals", new { area = "Individuals" });
                }
                else
                {
                    return RedirectToAction("UserProfile", "Individuals", new { area = "Individuals" });
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