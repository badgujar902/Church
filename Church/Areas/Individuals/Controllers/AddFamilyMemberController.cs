using Church.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Church.Areas.Individuals.Controllers
{
    public class AddFamilyMemberController : Controller
    {
        CHC_NewEntities dbcontext = new CHC_NewEntities();
        DateTime TodaysDate = DateTime.Now;
        string LoginMachinId = Dns.GetHostName();
        // GET: Individuals/AddFamilyMember
        #region MailView AddFamilyMember
        public ActionResult AddFamilyMember()
        {
            try
            {
                if (Session["U_Fid"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
                var MemberFId = Session["U_Fid"];
                int MemberFid = Convert.ToInt32(MemberFId);
                var MemberchurchId = dbcontext.MAS_UID.Where(X => X.MAS_INDVSL_FID == MemberFid).FirstOrDefault();
                var church = dbcontext.MAS_CHC.Where(X => X.Status == true && X.FID == MemberchurchId.MAS_CHC_FID).FirstOrDefault();
                ViewBag.ChurchList = church;
                return View();
            }
            catch (Exception ex)
            {

                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }

        }
        #endregion


        #region IsExistsData
        [HttpGet]
        public ActionResult IsExistsData(string Email, string MobileNumber)
        {
            try
            {
                if (Session["U_Fid"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
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
                    var GetEmail = dbcontext.MAS_INDVSL.Where(X => X.IND_Email == Email).FirstOrDefault();
                    var GetMobileNumber = dbcontext.MAS_INDVSL.Where(X => X.IND_Mob == MobileNumber).FirstOrDefault();

                    //if (Data != null)
                    //{
                    //    if (Data.AdharNo != null)
                    //    {
                    //        error = 1;
                    //        TempData["Message"] = "Enter Valid Adhar No  !";
                    //        TempData["Icon"] = "error";
                    //        return Json(new { Message = TempData["Message"], Icon = TempData["Icon"] }, JsonRequestBehavior.AllowGet);
                    //    }
                    //}
                    if (GetEmail != null)
                    {
                        if (GetEmail.IND_Email != null)
                        {
                            error = 1;
                            TempData["Message"] = "Email Id can not be duplicate";
                            TempData["Icon"] = "error";
                            return Json(new { Message = TempData["Message"], Icon = TempData["Icon"] }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    if (GetMobileNumber != null)
                    {
                        if (GetMobileNumber.IND_Mob != null)
                        {
                            error = 1;
                            TempData["Message"] = "Mobile Number Can not be duplicate ";
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
        #endregion

        #region AddFamilyMember
        [HttpPost]
        public ActionResult AddFamilyMember(Individual individual1, HttpPostedFileBase file, string U_ID23, string U_Pass23)
        {
            try
            {
                if (Session["U_Fid"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
                string LoginmachinIp = Dns.GetHostByName(LoginMachinId).AddressList[0].ToString();
                int save = 0;
                var MemberFId = Session["U_Fid"];
                var IndvslChurchFid = Session["IndvslUserCurchId"];
                int MemberFID = Convert.ToInt32(MemberFId);
                int IndvslChurchFID = Convert.ToInt32(IndvslChurchFid);

                HttpPostedFile files = System.Web.HttpContext.Current.Request.Files["file"];

                var MemeberPostCode = (from data in dbcontext.MAS_INDVSL where data.FID == MemberFID && data.MAS_CHC_FID == IndvslChurchFID && data.Deactivate == false select data).FirstOrDefault();

                if (individual1 != null)
                {
                    MAS_INDVSL individual2 = new MAS_INDVSL();
                    individual2.FDate = Convert.ToDateTime(TodaysDate);
                    individual2.MacID = LoginMachinId;
                    individual2.MacIP = LoginmachinIp;
                    individual2.StateFid = MemeberPostCode.StateFid;
                    individual2.City = MemeberPostCode.City;
                    individual2.Area = MemeberPostCode.Area;
                    individual2.MAS_CHC_FID = Convert.ToInt32(IndvslChurchFid);
                    individual2.IND_Name = individual1.IND_Name;
                    individual2.IND_Mob = individual1.IND_Mob;
                    individual2.IND_Email = individual1.IND_Email;
                    individual2.IND_Address = individual1.IND_Address;
                    individual2.IND_DOJ = DateTime.Now;
                    individual2.IND_DOB = individual1.IND_DOB;
                    //individual2.AdharNo = individual1.MemberAdharNo;
                    individual2.Gender = individual1.Gender;
                    individual2.IND_ReffName = individual1.IND_ReffName;
                    individual2.Enroll_Type = "Self";
                    var Enroll_ID = ((from db in dbcontext.MAS_INDVSL select (int?)db.Enroll_ID).Max() ?? 0) + 1;
                    individual2.Enroll_ID = Enroll_ID;
                    individual2.Status = true;
                    individual2.Deactivate = false;

                    var filelctn = individual1.IND_Mob;
                    string MoveLocation = @"C:\StealthChurch\RegistrationData\\" + filelctn + "\\";

                    if (files.ContentLength > 0)
                    {
                        if (!Directory.Exists(MoveLocation))
                        {
                            Directory.CreateDirectory(MoveLocation);
                        }
                        files.SaveAs(MoveLocation + Path.GetFileName((files.FileName)));
                        individual2.IND_Image = Convert.ToString(MoveLocation + Path.GetFileName(files.FileName));
                    }
                    dbcontext.MAS_INDVSL.Add(individual2);
                    save = dbcontext.SaveChanges();                

                    if (save != 0)
                    {

                        MAS_INDVFMLY IndvslFamily = new MAS_INDVFMLY();
                        var MAS_INDVSLFamily_fid = (from data in dbcontext.MAS_INDVSL where data.Status == true && data.IND_Mob == individual1.IND_Mob select data.FID).Max();
                        int indFid = Convert.ToInt32(MAS_INDVSLFamily_fid);

                        IndvslFamily.Fdate = DateTime.Now;
                        IndvslFamily.Mid = LoginMachinId;
                        IndvslFamily.Mip = LoginmachinIp;
                        IndvslFamily.MAS_INDVSL_Fid = Convert.ToInt32(MemberFId);
                        IndvslFamily.Member_id = indFid;
                        IndvslFamily.Relation = individual1.Reletion;
                        IndvslFamily.Active = true;

                        dbcontext.MAS_INDVFMLY.Add(IndvslFamily);
                        save = dbcontext.SaveChanges();
                    }
                }


                if (save != 0)
                {
                    TempData["Message"] = "Family Member Added Successfully !";
                    TempData["Icon"] = "success";
                }
                else
                {
                    TempData["Message"] = "Family Member Not Added Successfully !";
                    TempData["Icon"] = "error";
                }

                return RedirectToAction("AddFamilyMember", "AddFamilyMember", new { area = "Individuals" });
            }
            catch (Exception ex)
            {
                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }
        }
        #endregion


        #region FamilyMemberList
        public ActionResult FamilyMemberList()
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

                var GetFamilyMemberList = dbcontext.sp_List_INDVSLFamilyMember(INDVSLFId, ChurchFid).ToList();
                ViewBag.FamilyMemberList = GetFamilyMemberList;

                return View();
            }
            catch (Exception ex)
            {
                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }
        }
        #endregion
        #region UpdateFamilyMember
        public ActionResult UpdateFamilyMember(int? id)
        {
            try
            {
                if (Session["U_Fid"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }


                MAS_INDVSL FamilyMemberDetails = new MAS_INDVSL();

                FamilyMemberDetails = (from data in dbcontext.MAS_INDVSL where data.FID == id select data).FirstOrDefault();

                string imageNameWithExtension = Path.GetFileName(FamilyMemberDetails.IND_Image);
                ViewBag.file = imageNameWithExtension;

                return View(FamilyMemberDetails);

            }
            catch (Exception ex)
            {
                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }
        }

        public ActionResult SaveUpdateFamilyMember(MAS_INDVSL MemberDetails, HttpPostedFileBase file)
        {
            try
            {
                if (Session["U_Fid"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
                HttpPostedFile files = System.Web.HttpContext.Current.Request.Files["file"];
                int update = 0;
                if (MemberDetails != null)
                {
                    var updatData = (from data in dbcontext.MAS_INDVSL where data.FID == MemberDetails.FID select data).FirstOrDefault();
                    updatData.IND_Name = MemberDetails.IND_Name;
                    updatData.Gender = MemberDetails.Gender;
                    updatData.IND_Mob = MemberDetails.IND_Mob;
                    updatData.IND_DOB = MemberDetails.IND_DOB;
                    updatData.IND_Email = MemberDetails.IND_Email;
                    updatData.IND_Address = MemberDetails.IND_Address;
                    //updatData.AdharNo = MemberDetails.AdharNo;

                    var filelctn = MemberDetails.IND_Mob;
                    string MoveLocation = @"C:\StealthChurch\RegistrationData\\" + filelctn + "\\";
                    if (files != null)
                    {
                        if (files.ContentLength > 0)
                        {
                            if (!Directory.Exists(MoveLocation))
                            {
                                Directory.CreateDirectory(MoveLocation);
                            }
                            files.SaveAs(MoveLocation + Path.GetFileName((files.FileName)));
                            updatData.IND_Image = Convert.ToString(MoveLocation + Path.GetFileName(files.FileName));
                        }

                    }

                    update = dbcontext.SaveChanges();
                }
                if (update >= 0)
                {
                    TempData["Message"] = " Updated Successfully";
                    TempData["Icon"] = "success";
                    return RedirectToAction("FamilyMemberList", "AddFamilyMember", new { area = "Individuals" });
                }
                else
                {
                    TempData["Message"] = "Not Update";
                    TempData["Icon"] = "error";
                    return RedirectToAction("FamilyMemberList", "AddFamilyMember", new { area = "Individuals" });
                }
            }
            catch (Exception ex)
            {

                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }
        }
        #endregion

        public ActionResult DeleteFamilyMember(int? FamilyMemberFid)
        {
            try
            {
                if (Session["U_Fid"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
                var MemberFid = Session["U_Fid"];
                int MemberFId = Convert.ToInt32(MemberFid);
                var UpdateData = (from data in dbcontext.MAS_INDVFMLY where data.Member_id == FamilyMemberFid && data.MAS_INDVSL_Fid == MemberFId select data).FirstOrDefault();
                UpdateData.Active = false;
                int i = dbcontext.SaveChanges();
                if (i >= 0)
                {
                    var UpdateData1 = (from data in dbcontext.MAS_INDVSL where data.FID == FamilyMemberFid select data).FirstOrDefault();
                    UpdateData1.Deactivate = true;
                    int save = dbcontext.SaveChanges();
                    if (save >= 0)
                    {
                        TempData["Message"] = "Family Member has been Deleted";
                        TempData["Icon"] = "success";
                        return RedirectToAction("FamilyMemberList", "AddFamilyMember", new { area = "Individuals" });
                    }
                    else
                    {
                        TempData["Message"] = "Family Member Not Deleted";
                        TempData["Icon"] = "error";
                        return RedirectToAction("FamilyMemberList", "AddFamilyMember", new { area = "Individuals" });
                    }
                }
                else
                {
                    TempData["Message"] = "Prayer Request Not Deleted Successfully";
                    TempData["Icon"] = "error";
                    return RedirectToAction("PrayerRequestList", "AddFamilyMember", new { area = "Individuals" });
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