using Church.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Church.Areas.Admin.Controllers
{
    public class RegistrationRequstListController : Controller
    {
        CHC_NewEntities dbcontext = new CHC_NewEntities();
        DateTime TodaysDate = DateTime.Now;
        string LoginMachinId = Dns.GetHostName();
        // GET: Admin/RegistrationRequstList
        public ActionResult RegistrationRequstList()
        {
            try
            {
                if (Session["Admin"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }      

                var GetRistrationList = (from data in dbcontext.MAS_INDVSL where data.Status == true && data.Enroll_Type == "Self" && data.Deactivate == false orderby data.FID descending select (data)).ToList();


                ViewBag.RistrationList = GetRistrationList;
                if (GetRistrationList.Count() == 0)
                {
                    TempData["Message"] = "No Pending Registration Request";
                    TempData["Icon"] = "warning";
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