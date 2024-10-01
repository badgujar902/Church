using Church.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Church.Areas.Individuals.Controllers
{
    public class PrayerRequestController : Controller
    {
        CHC_NewEntities dbcontext = new CHC_NewEntities();
        DateTime TodaysDate = DateTime.Now;
        string LoginMachinId = Dns.GetHostName();
        // GET: Individuals/PrayerRequest
        public ActionResult UpdatePrayerRequest(int ? FID)
        {
            try
            {
                if (Session["U_Fid"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
                MAS_PrayerReq PrayerRequest = new MAS_PrayerReq();

                PrayerRequest = (from data in dbcontext.MAS_PrayerReq where data.FID == FID select data).FirstOrDefault();
                if (PrayerRequest.Req_Status == true)
                {
                    TempData["Message"] = "Your Request will not get Update";
                    TempData["MesgTitle"] = "Request Already Approved";
                    TempData["Icon"] = "warning";
                    return RedirectToAction("PrayerRequestList", "Individuals", new { area = "Individuals" });
                }
                return View(PrayerRequest);
            }
            catch (Exception ex)
            {
                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }
          
        }
        [HttpPost]
        public ActionResult SaveUpdatePrayerRequest(MAS_PrayerReq Request)
        {
            try
            {
                if (Session["U_Fid"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
                var updatedata = (from data in dbcontext.MAS_PrayerReq where data.FID == Request.FID && data.Deactivate==false select data).FirstOrDefault();

                if(updatedata.Req_Status==true)
                {
                    TempData["Message"] = "Your Request Already Approved You Can't Update";
                    TempData["Icon"] = "error";
                    return RedirectToAction("PrayerRequestList", "Individuals", new { area = "Individuals"});
                }
                else
                {
                    updatedata.PrayerRemark = Request.PrayerRemark;
                    updatedata.Req_Date = Request.Req_Date;

                    int i = dbcontext.SaveChanges();
                    if (i >= 0)
                    {
                        TempData["Message"] = "Prayer Request Updated Successfully";
                        TempData["Icon"] = "success";
                        return RedirectToAction("UpdatePrayerRequest", "PrayerRequest", new { area = "Individuals", FID= Request.FID });
                    }
                    else
                    {
                        TempData["Message"] = "Prayer Request Not Updated Successfully";
                        TempData["Icon"] = "error";
                        return RedirectToAction("UpdatePrayerRequest", "PrayerRequest", new { area = "Individuals", FID = Request.FID });
                    }
                }

               
          
            }
            catch (Exception ex)
            {
                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }
        }

        public ActionResult DeletePrayerRequest(int? FID)
        {
            try
            {
                if (Session["U_Fid"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
                int i = 0;
                var MemberFid = Session["U_Fid"];
                int MemberFId = Convert.ToInt32(MemberFid);
                var UpdateData = (from data in dbcontext.MAS_PrayerReq where data.FID == FID && data.Req_ID == MemberFId select data).FirstOrDefault();

                if(UpdateData.Req_Status==true)
                {
                    TempData["Message"] = "Your Request will not get deleted";
                    TempData["MesgTitle"] = "Request Already Approved";
                    TempData["Icon"] = "warning";
                    return RedirectToAction("PrayerRequestList", "Individuals", new { area = "Individuals" });
                }
                else
                {
                    UpdateData.Deactivate = true;
                    i = dbcontext.SaveChanges();
                }
            
            
                if (i >= 1)
                {
                    TempData["Message"] = "Prayer Request Deleted";
                    TempData["Icon"] = "success";
                    return RedirectToAction("PrayerRequestList", "Individuals", new { area = "Individuals" });
                }
                else
                {
                    TempData["Message"] = "Prayer Request Not Deleted ";
                    TempData["Icon"] = "error";
                    return RedirectToAction("PrayerRequestList", "Individuals", new { area = "Individuals" });
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