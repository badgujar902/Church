using Church.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Church.Areas.Leader.Controllers
{
    public class LeaderRegisterdMembersController : Controller
    {
        CHC_NewEntities dbcontext = new CHC_NewEntities();
        DateTime TodaysDate = DateTime.Now;
        string LoginMachinId = Dns.GetHostName();
        // GET: Leader/LeaderRegisterdMembers
        public ActionResult LeaderRegisterdMembers()
        {
            return View();
        }

        public ActionResult LeaderRegisterdMembersList()
        {
            try
            {
                if (Session["LeaderDesignation"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
                var LeaderDesignation = Session["LeaderDesignation"];

                var HeadLeaderCurchId = Session["HeadLeaderCurchId"];
                int HeadLeaderCurchFId = Convert.ToInt32(HeadLeaderCurchId);

                var GetLeaderRegistredMember = dbcontext.sp_List_LeaderRegistredMembersList(HeadLeaderCurchFId).ToList();              

                if(GetLeaderRegistredMember.Count==0)
                {
                    ViewBag.LeaderRegistredMember = "";
                }else
                {
                    ViewBag.LeaderRegistredMember = GetLeaderRegistredMember;
                }

                return View();
            }
            catch ( Exception ex)
            {

                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }
        }
    }
}