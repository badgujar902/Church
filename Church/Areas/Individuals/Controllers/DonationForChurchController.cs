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
    public class DonationForChurchController : Controller
    {
        CHC_NewEntities dbcontext = new CHC_NewEntities();
        DateTime TodaysDate = DateTime.Now;
        string LoginMachinId = Dns.GetHostName();
        // GET: Individuals/DonationForChurch
        public ActionResult Donation()
        {
            try
            {
                if (Session["U_Fid"] == null)
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

        public ActionResult SaveDonation( Mas_Donation donation, HttpPostedFileBase file)
        {
            try
            {
                if (Session["U_Fid"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
                int save = 0;
                HttpPostedFile files = System.Web.HttpContext.Current.Request.Files["file"];
                var userFid = Session["U_Fid"];
                int MemberFid = Convert.ToInt32(userFid);
                var userChurchId = Session["IndvslUserCurchId"];
                int MemberChurchFid = Convert.ToInt32(userChurchId);
                var LoginmachinIp = Dns.GetHostByName(LoginMachinId).AddressList[0].ToString();
                Mas_Donation Donation = new Mas_Donation();

                Donation.MacID = LoginMachinId;
                Donation.MacIP = LoginmachinIp;
                Donation.FDate = DateTime.Now;
                Donation.MemberFId = MemberFid;
                Donation.ChurchFId = MemberChurchFid;
                Donation.DonationDate = donation.DonationDate;
                Donation.Purpose = donation.Purpose;
                Donation.Amount = donation.Amount;
                Donation.Status = true;
                Donation.Deactivate = false;

                var filelctn = MemberFid;
                string MoveLocation = @"C:\StealthChurch\DonationAttechment\\" + filelctn + "\\";

                if (files.ContentLength > 0)
                {
                    if (!Directory.Exists(MoveLocation))
                    {
                        Directory.CreateDirectory(MoveLocation);
                    }
                    files.SaveAs(MoveLocation + Path.GetFileName((files.FileName)));
                    Donation.DonationAttechment = Convert.ToString(MoveLocation + Path.GetFileName(files.FileName));
                }

                dbcontext.Mas_Donation.Add(Donation);
                save = dbcontext.SaveChanges();
                if(save!=0)
                {
                    TempData["Message"] = "Donation Save Successfully !";
                    TempData["Icon"] = "success";
                    return RedirectToAction("Donation", "DonationForChurch", new { area = "Individuals" });
                }
                else
                {
                    TempData["Message"] = "Donation Not Save Successfully !";
                    TempData["Icon"] = "error";
                    return RedirectToAction("Donation", "DonationForChurch", new { area = "Individuals" });
                }

            }
            catch (Exception ex)
            {
                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }
        }


        public ActionResult DonationList()
        {
            try
            {
                if (Session["U_Fid"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
                var userFid = Session["U_Fid"];
                int MemberFid = Convert.ToInt32(userFid);
                var userChurchId = Session["IndvslUserCurchId"];
                int MemberChurchFid = Convert.ToInt32(userChurchId);

                var DonationData = (from data in dbcontext.Mas_Donation where data.ChurchFId == MemberChurchFid && data.MemberFId == MemberFid && data.Deactivate==false orderby data.FId descending select data).ToList();
               

                if(DonationData.Count==0)
                {
                    ViewBag.DonationList = "";
                }else
                {
                    ViewBag.DonationList = DonationData;
                }
                return View();
            }
            catch (Exception ex)
            {
                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }
        }

        #region SaveUpdate
        public ActionResult UpdateDonation(int? FId, int? ChurchFId)
        {
            try
            {
                if (Session["U_Fid"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }

                Mas_Donation Donation = new Mas_Donation();

                Donation = (from data in dbcontext.Mas_Donation where data.FId == FId && data.ChurchFId == ChurchFId && data.Deactivate == false select data).FirstOrDefault();

                if (Donation.Status == false)
                {
                    TempData["Message"] = "Donation cannot be updated ";
                    TempData["MesgTitle"] = "Donation already Verified";
                    TempData["Icon"] = "warning";
                    return RedirectToAction("DonationList", "DonationForChurch", new { area = "Individuals" });
                }

                string Attechment = Path.GetFileName(Donation.DonationAttechment);
                ViewBag.file = Attechment;
                return View(Donation);
            }
            catch (Exception ex)
            {
                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }
        }
        [HttpPost]
        public ActionResult SaveUpdate(Mas_Donation Donation, HttpPostedFileBase file)
        {
            try
            {
                if (Session["U_Fid"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
                var userFid = Session["U_Fid"];
                int MemberFid = Convert.ToInt32(userFid);

                int update = 0;
                HttpPostedFile files = System.Web.HttpContext.Current.Request.Files["file"];

                var UpdateDonation = (from data in dbcontext.Mas_Donation where data.FId == Donation.FId && data.ChurchFId == Donation.ChurchFId && data.Deactivate == false select data).FirstOrDefault();


                UpdateDonation.DonationDate = Donation.DonationDate;
                UpdateDonation.Amount = Donation.Amount;
                UpdateDonation.Purpose = Donation.Purpose;

                var filelctn = MemberFid;
                string MoveLocation = @"C:\StealthChurch\DonationAttechment\\" + filelctn + "\\";

                if (files.ContentLength > 0)
                {
                    if (!Directory.Exists(MoveLocation))
                    {
                        Directory.CreateDirectory(MoveLocation);
                    }
                    files.SaveAs(MoveLocation + Path.GetFileName((files.FileName)));
                    UpdateDonation.DonationAttechment = Convert.ToString(MoveLocation + Path.GetFileName(files.FileName));
                }
                update = dbcontext.SaveChanges();

                if (update >= 0)
                {
                    TempData["Message"] = "Donation Updated Successfully !";
                    TempData["Icon"] = "success";
                    return RedirectToAction("DonationList", "DonationForChurch", new { area = "Individuals" });
                }
                else
                {
                    TempData["Message"] = "Donation not Updated Successfully !";
                    TempData["Icon"] = "error";
                    return RedirectToAction("DonationList", "DonationForChurch", new { area = "Individuals" });
                }


            }
            catch (Exception ex)
            {
                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }
        }
        #endregion


        #region DeleteDonation
        public ActionResult DeleteDonation(int? FId, int? ChurchFId)
        {
            try
            {
                if (Session["U_Fid"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
                int Delete = 0;
                var DeleteDonation = (from data in dbcontext.Mas_Donation where data.FId == FId && data.ChurchFId == ChurchFId && data.Deactivate == false select data).FirstOrDefault();
                if (DeleteDonation.Status == false)
                {
                    TempData["Message"] = "Donation cannot be Delete ";
                    TempData["MesgTitle"] = "Donation already Verified";
                    TempData["Icon"] = "warning";
                    return RedirectToAction("DonationList", "DonationForChurch", new { area = "Individuals" });
                }else
                {
                    DeleteDonation.Deactivate = true;
                     Delete = dbcontext.SaveChanges();
                }
             

                if (Delete != 0)
                {
                    TempData["Message"] = "Donation Successfully Deleted";
                    TempData["Icon"] = "success";
                    return RedirectToAction("DonationList", "DonationForChurch", new { area = "Individuals" });
                }
                else
                {
                    TempData["Message"] = "Donation not Deleted";
                    TempData["Icon"] = "success";
                    return RedirectToAction("DonationList", "DonationForChurch", new { area = "Individuals" });
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