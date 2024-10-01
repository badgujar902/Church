using Church.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Church.Areas.Admin.Controllers
{
    public class EnquiryListController : Controller
    {
        CHC_NewEntities dbcontext = new CHC_NewEntities();
        DateTime TodaysDate = DateTime.Now;
        string LoginMachinId = Dns.GetHostName();
        // GET: Admin/EnquiryList
        public ActionResult EnquiryList()
        {
            try
            {

                var GetEnquiry = dbcontext.sp_List_Enquiry(null,null,null,null).ToList();
                ViewBag.GetEnquiryList = GetEnquiry;
                if (GetEnquiry.Count == 0)
                {
                    TempData["Message"] = "No Pending Inquiries ";
                    TempData["Icon"] = "warning";
                    return RedirectToAction("Dashboard", "Leader", new { area = "Leader" });
                }
                return View();
            }
            catch (Exception ex)
            {
                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }
           
        }
    }
}