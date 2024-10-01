using Church.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Church.Areas.Individuals.Controllers
{
    public class UpdateDeleteEnquiryController : Controller
    {
        CHC_NewEntities dbcontext = new CHC_NewEntities();
        DateTime TodaysDate = DateTime.Now;
        string LoginMachinId = Dns.GetHostName();
        // GET: Individuals/UpdateDeleteEnquiry
        public ActionResult UpdateEnquiry(int? FID,int? CurchId)
        {
            try
            {
                if (Session["U_Fid"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
                Mas_Enquiry Enquiry = new Mas_Enquiry();
                Enquiry = (from data in dbcontext.Mas_Enquiry where data.FId == FID && data.CurchId == CurchId && data.Status == true && data.LeaderResponseStatus == true && data.Deactivate==false select data).FirstOrDefault();
                if(Enquiry==null)
                {
                    TempData["MesgTitle"] = "Inquiry has been responded";
                    TempData["Message"] = "Inquiry will not get updated";
                    TempData["Icon"] = "error";
                    return RedirectToAction("EnquiryList", "Individuals", new { area = "Individuals" });
                }
                else
                {
                    return View(Enquiry);
                }

           
            }
            catch (Exception ex)
            {
                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }
        
        }

        [HttpPost]
        public ActionResult saveUpdateEnquiry(string Subject,string Enquiry,int? FId,int? CurchId)
        {
            try
            {
                if (Session["U_Fid"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
                int update = 0;
                var updateEnquiry= (from data in dbcontext.Mas_Enquiry where data.FId == FId && data.CurchId == CurchId && data.Status == true && data.LeaderResponseStatus == true && data.Deactivate==false select data).FirstOrDefault();

                if(updateEnquiry!=null)
                {
                    updateEnquiry.Subject = Subject;
                    updateEnquiry.Enquiry = Enquiry;
                    update = dbcontext.SaveChanges();
                }
                if(update>=0)
                {
                    TempData["Message"] = "Inquiry Updated Successfully";
                    TempData["Icon"] = "success";
                    return RedirectToAction("EnquiryList", "Individuals", new { area = "Individuals" });
                }
                else
                {
                    TempData["Message"] = "Inquiry Not Updated Successfully";
                    TempData["Icon"] = "error";
                    return RedirectToAction("EnquiryList", "Individuals", new { area = "Individuals" });
                }

            
            }
            catch (Exception ex)
            {
                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }
        }

        public ActionResult DeleteEnquiry(int? FID,int? CurchId)
        {
            try
            {
                if (Session["U_Fid"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
                int save = 0;
                var DeleteEnquiry= (from data in dbcontext.Mas_Enquiry where data.FId == FID && data.CurchId == CurchId  && data.Deactivate == false select data).FirstOrDefault();

                if(DeleteEnquiry!=null)
                {
                    if(DeleteEnquiry.Status==false && DeleteEnquiry.LeaderResponseStatus==false)
                    {
                        TempData["Message"] = "Your Inquiry will not get deleted";
                        TempData["MesgTitle"] = "Inquiry has been responded";
                        TempData["Icon"] = "warning";
                        return RedirectToAction("EnquiryList", "Individuals", new { area = "Individuals" });
                    }else
                    {
                        DeleteEnquiry.Deactivate = true;
                        save = dbcontext.SaveChanges();
                    }
                }

                if(save!=0)
                {
                    TempData["Message"] = "Inquiry Deleted ";
                    TempData["Icon"] = "success";
                    return RedirectToAction("EnquiryList", "Individuals", new { area = "Individuals" });
                }else
                {
                    TempData["Message"] = "Inquiry Not Deleted ";
                    TempData["Icon"] = "error";
                    return RedirectToAction("EnquiryList", "Individuals", new { area = "Individuals" });
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