using Church.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Church.Areas.Leader.Controllers
{
    public class UpdateLeaderRegisterMemberController : Controller
    {
        CHC_NewEntities dbcontext = new CHC_NewEntities();
        DateTime TodaysDate = DateTime.Now;
        string LoginMachinId = Dns.GetHostName();
        // GET: Leader/UpdateLeaderRegisterMember
        public ActionResult UpdateMember(int? FID,int? MAS_CHC_FID)
        {
            try
            {
                if (Session["LeaderDesignation"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
                MAS_INDVSL indvsl = new MAS_INDVSL();

                indvsl = (from data in dbcontext.MAS_INDVSL where data.FID == FID && data.MAS_CHC_FID == MAS_CHC_FID && data.Deactivate == false select data).FirstOrDefault();

                var userIdPassowrd = (from data in dbcontext.MAS_UID where data.MAS_INDVSL_FID == FID && data.MAS_CHC_FID == MAS_CHC_FID && data.U_Status == true select data).FirstOrDefault();
                ViewBag.Password = userIdPassowrd.U_Pass;
                ViewBag.UserId = userIdPassowrd.U_ID;

                //string imageName = Path.GetFileNameWithoutExtension(indvsl.IND_Image);
                string imageNameWithExtension = Path.GetFileName(indvsl.IND_Image);
                ViewBag.file = "Upload File: "+imageNameWithExtension;

                return View(indvsl);
            }
            catch (Exception ex)
            {
                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }
         
        }
        [HttpGet]
        public ActionResult IsExistsData(string Email)
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
                    else
                    {
                        return Json(true, JsonRequestBehavior.AllowGet);
                    }
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
        public ActionResult SaveUpdateMember(MAS_INDVSL INDVSL,string UserId,string Password, HttpPostedFileBase file)
        {
            try
            {
                if (Session["LeaderDesignation"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
                int update = 0;
                var UpdateRegisterMember = (from data in dbcontext.MAS_INDVSL where data.FID == INDVSL.FID && data.MAS_CHC_FID == INDVSL.MAS_CHC_FID && data.Deactivate == false select data).FirstOrDefault();
                HttpPostedFile files = System.Web.HttpContext.Current.Request.Files["file"];  
                if(UpdateRegisterMember!=null)
                {

                    //UpdateRegisterMember.AdharNo = INDVSL.AdharNo;
                    UpdateRegisterMember.IND_Address = INDVSL.IND_Address;
                    UpdateRegisterMember.IND_DOB = INDVSL.IND_DOB;
                    UpdateRegisterMember.IND_Email = INDVSL.IND_Email;
                    UpdateRegisterMember.IND_Mob = INDVSL.IND_Mob;
                    UpdateRegisterMember.IND_Name = INDVSL.IND_Name;
                    UpdateRegisterMember.Gender = INDVSL.Gender;

                    var filelctn = INDVSL.IND_Mob;
                    string MoveLocation = @"C:\StealthChurch\RegistrationData\\" + filelctn + "\\";
                    if(files!=null)
                    {
                        if (files.ContentLength > 0)
                        {
                            if (!Directory.Exists(MoveLocation))
                            {
                                Directory.CreateDirectory(MoveLocation);
                            }
                            files.SaveAs(MoveLocation + Path.GetFileName((files.FileName)));
                            UpdateRegisterMember.IND_Image = Convert.ToString(MoveLocation + Path.GetFileName(files.FileName));
                        }

                    }

                    update = dbcontext.SaveChanges();
                }      
               
                if(update>=0)
                {
                    var updateUserIdPassword = (from data in dbcontext.MAS_UID where data.MAS_INDVSL_FID == INDVSL.FID && data.MAS_CHC_FID == INDVSL.MAS_CHC_FID && data.U_Status == true select data).FirstOrDefault();

                    updateUserIdPassword.U_ID = UserId;
                    updateUserIdPassword.U_Pass = Password;

                    int save = dbcontext.SaveChanges();
                    if(save>=0)
                    {
                        TempData["Message"] = "Member Updated successfully";
                        TempData["Icon"] = "success";
                        return RedirectToAction("LeaderRegisterdMembersList", "LeaderRegisterdMembers", new { area = "Leader" });
                    }
                    else
                    {
                        TempData["Message"] = "Member Not Updated successfully";
                        TempData["Icon"] = "error";
                        return RedirectToAction("LeaderRegisterdMembersList", "LeaderRegisterdMembers", new { area = "Leader" });
                    }

                    
                }
                else
                {
                    TempData["Message"] = "Member Not Updated successfully";
                    TempData["Icon"] = "error";
                    return RedirectToAction("LeaderRegisterdMembersList", "LeaderRegisterdMembers", new { area = "Leader" });
                }
         
            }
            catch (Exception ex)
            {
                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }
        }

        public ActionResult DeleteMember(int? FID, int? MAS_CHC_FID)
        {
            try
            {
                if (Session["LeaderDesignation"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
                int Delete = 0;
                var DeleteMember = (from data in dbcontext.MAS_INDVSL where data.FID == FID && data.MAS_CHC_FID == MAS_CHC_FID && data.Deactivate == false select data).FirstOrDefault();
                DeleteMember.Deactivate = true;

                Delete = dbcontext.SaveChanges();
                if(Delete!=0)
                {
                    var DeleteUserIdPass = (from data in dbcontext.MAS_UID where data.U_Status == true && data.MAS_INDVSL_FID == FID && data.MAS_CHC_FID == MAS_CHC_FID select data).FirstOrDefault();
                    DeleteUserIdPass.U_Status = false;
                    int Delete1 = dbcontext.SaveChanges();
                    if (Delete1>=0)
                    {
                        TempData["Message"] = "Member Deleted successfully";
                        TempData["Icon"] = "success";
                        return RedirectToAction("LeaderRegisterdMembersList", "LeaderRegisterdMembers", new { area = "Leader" });
                    }else
                    {
                        TempData["Message"] = "Member Not Deleted successfully";
                        TempData["Icon"] = "error";
                        return RedirectToAction("LeaderRegisterdMembersList", "LeaderRegisterdMembers", new { area = "Leader" });
                    }
                  
                }
                else
                {
                    TempData["Message"] = "Member Not Deleted successfully";
                    TempData["Icon"] = "error";
                    return RedirectToAction("LeaderRegisterdMembersList", "LeaderRegisterdMembers", new { area = "Leader" });
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