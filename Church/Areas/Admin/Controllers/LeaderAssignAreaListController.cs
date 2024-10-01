using Church.Areas.Admin.Models;
using Church.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Church.Areas.Admin.Controllers
{
    public class LeaderAssignAreaListController : Controller
    {
        CHC_NewEntities dbcontext = new CHC_NewEntities();
        DateTime TodaysDate = DateTime.Now;
        string LoginMachinId = Dns.GetHostName();
        // GET: Admin/LeaderAssignAreaList
        public ActionResult LeaderAssignAreaList()
        {
            try
            {
                if (Session["Admin"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
                var GetList = dbcontext.sp_List_AssignArea(null, null).ToList();
                ViewBag.GetAssignList = GetList;

                //var GetLeader = (from data in dbcontext.MAS_LEADER where data.Status==true select new BindDrop { Id = data.FID, Name = data.Lead_Name }).ToList();
                //ViewBag.GEtLeaderName = GetLeader;

                //var GetLeaderArea = (from data in dbcontext.MAS_LEADER where data.Status == true select new BindDrop { Id = data.FID, Name = data.Lead_Name }).ToList();
                //ViewBag.GEtLeaderName = GetLeader;
            }
            catch (Exception ex)
            {
                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }
            return View();
        }
    }
}