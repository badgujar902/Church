using Church.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Church.Areas.Leader.Controllers
{
    public class DonationController : Controller
    {
        CHC_NewEntities dbcontext = new CHC_NewEntities();
        DateTime TodaysDate = DateTime.Now;
        string LoginMachinId = Dns.GetHostName();
        // GET: Leader/Donation
        public ActionResult DonationList(int? MemberFid)
        {
            try
            {          
                if (Session["LeaderFId"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
                var curchFId = Session["LeaderCurchId"];
                int LeaderChurchId = Convert.ToInt32(curchFId);

                if (MemberFid!=null)
                {
                    var DonationData = (from data in dbcontext.Mas_Donation where data.ChurchFId == LeaderChurchId && data.MemberFId== MemberFid  && data.Deactivate == false && data.Status == true select data).ToList();
                    if(DonationData.Count()==0)
                    {
                        TempData["Message"] = "No pending donation for verification";
                        TempData["Icon"] = "error";
                        return RedirectToAction("ChurchMemberDetails", "ChurchMemberDetails",new {area= "Leader", MemberFId= MemberFid });
                    }

                    var MemberName = (from data in dbcontext.MAS_INDVSL where data.FID == MemberFid && data.Deactivate == false select data).FirstOrDefault();
                    ViewBag.MemeberName = MemberName.IND_Name;
                  
                    if (DonationData.Count() == 0 || DonationData == null)
                    {
                        ViewBag.DonationList = "";
                    }
                    else
                    {
                        ViewBag.DonationList = DonationData;
                    }

                    var BasUrl = (from data in dbcontext.DocumentBaseUrls select data).FirstOrDefault();
                    if (BasUrl != null)
                    {
                        ViewBag.BasUrl = BasUrl.BaseUrl;
                    }
                    else
                    {
                        ViewBag.BasUrl = "";
                    }
                }
                else
                {
                    //var DonationData = (from data in dbcontext.Mas_Donation where data.ChurchFId == LeaderChurchId && data.Deactivate == false && data.Status == true select data).ToList();
                    var DonationData = dbcontext.sp_List_Donation(1, LeaderChurchId,null, null).ToList();
                 
                    if (DonationData.Count() == 0 || DonationData == null)
                    {
                        ViewBag.DonationList = "";
                    }
                    else
                    {
                        ViewBag.DonationList = DonationData;
                    }

                    var BasUrl = (from data in dbcontext.DocumentBaseUrls select data).FirstOrDefault();
                    if (BasUrl != null)
                    {
                        ViewBag.BasUrl = BasUrl.BaseUrl;
                    }
                    else
                    {
                        ViewBag.BasUrl = "";
                    }

                    var verifiedDonationList = dbcontext.sp_List_Donation(0, LeaderChurchId, null, null).ToList();
                 
                    if (verifiedDonationList.Count() == 0 || verifiedDonationList == null)
                    {
                        ViewBag.VerifiedDonationList = "";
                    }
                    else
                    {
                        ViewBag.VerifiedDonationList = verifiedDonationList;
                    }

                    var BasUrll = (from data in dbcontext.DocumentBaseUrls select data).FirstOrDefault();
                    if (BasUrll != null)
                    {
                        ViewBag.BasUrll = BasUrll.BaseUrl;
                    }
                    else
                    {
                        ViewBag.BasUrll = "";
                    }
                }


                return View();
            }
            catch (Exception ex)
            {
                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }
        
        }

        public ActionResult DonationVerified(int? FId,int? ChurchFId)
        {
            try
            {
                if (Session["LeaderFId"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }

                var updateStatus = (from data in dbcontext.Mas_Donation where data.FId == FId && data.ChurchFId == ChurchFId && data.Deactivate == false && data.Status==true select data).FirstOrDefault();

                updateStatus.Status = false;
                int Verified = dbcontext.SaveChanges();

                if(Verified!=0)
                {
                    TempData["Message"] = "Donation has been Verified";
                    TempData["Icon"] = "success";
                    return RedirectToAction("DonationList", "Donation", new { area = "Leader" });
                }else
                {
                    TempData["Message"] = "Donation has been not Verified";
                    TempData["Icon"] = "error";
                    return RedirectToAction("DonationList", "Donation", new { area = "Leader" });
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