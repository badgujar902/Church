using Church.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Church.Areas.Leader.Controllers
{
    public class ChurchMemberDetailsController : Controller
    {
        CHC_NewEntities dbcontext = new CHC_NewEntities();
        DateTime TodaysDate = DateTime.Now;
        string LoginMachinId = Dns.GetHostName();
        // GET: Leader/ChurchMemberDetails
        public ActionResult ChurchMemberDetails(int? MemberFId, int? MAS_CHC_FID)
        {
            try
            {
                var LeaderDesignation = Session["LeaderDesignation"];


                if (LeaderDesignation.ToString() == "Head Leader")
                {
                    if (Session["HeadLeader_U_Fid"] == null)
                    {
                        return RedirectToAction("Login", "Home", new { area = "" });
                    }
                    var HeadLeaderFId = Session["HeadLeader_U_Fid"];
                    var HeadLeadercurchFId = Session["HeadLeaderCurchId"];
                    int HeadLeaderCurchFId = Convert.ToInt32(HeadLeadercurchFId);
                    int HeadLeader_FId = Convert.ToInt32(HeadLeaderFId);
                
                    var MemberPrayerRequest = (from data in dbcontext.MAS_PrayerReq where data.Deactivate == false && data.Req_ID == MemberFId && data.Req_Status == false && data.MAS_CHC_FID == HeadLeaderCurchFId select data).ToList();
                    ViewBag.MemberPendingPrayerRequest = MemberPrayerRequest.Count();

                    var LeaderStateFid = (from data in dbcontext.MAS_LEADER where data.FID == HeadLeader_FId && data.Status == true select data).FirstOrDefault();
                    //var MemberEnquiryRequest = (from data in dbcontext.Mas_Enquiry where data.Status == true && data.MemberFId == MemberFId && data.CurchId== MAS_CHC_FID && (data.Status = 1 && data.LeaderResponseStatus = 1
                    //    or data.Status = 0 and data.LeaderResponseStatus = 1) select data).ToList();
                    var MemberEnquiryRequest = (from data in dbcontext.Mas_Enquiry
                                                where data.Status == true
                                                      && data.MemberFId == MemberFId
                                                      && data.CurchId == MAS_CHC_FID
                                                      && ((data.Status == true && data.LeaderResponseStatus == true)
                                                          || (data.Status == false && data.LeaderResponseStatus == true))
                                                select data).ToList();
                    //var MemberEnquiryRequest = dbcontext.sp_List_Enquiry(HeadLeaderCurchFId, LeaderDesignation.ToString(), null, LeaderStateFid.StateFid).ToList();
                    ViewBag.MemberPendingEnquiryRequest = MemberEnquiryRequest.Count();

                    var DonationAmount = (from data in dbcontext.Mas_Donation where data.MemberFId == MemberFId && data.ChurchFId == HeadLeaderCurchFId && data.Status == true && data.Deactivate == false select data).ToList();
                    ViewBag.Donation = DonationAmount.Count();


                    if (MAS_CHC_FID == null)
                    {
                        var ChurchMember = (from data in dbcontext.MAS_INDVSL where data.FID == MemberFId && data.MAS_CHC_FID == HeadLeaderCurchFId && data.Deactivate == false select data).FirstOrDefault();


                        var MemberUserIDPassword = (from data1 in dbcontext.MAS_UID where data1.MAS_INDVSL_FID == MemberFId && data1.MAS_CHC_FID == HeadLeaderCurchFId && data1.U_Status == true select data1).FirstOrDefault();


                        if (MemberUserIDPassword == null)
                        {
                            ViewBag.UserId = "";
                            ViewBag.UserPass = "";

                            ViewBag.MemberDetails = "";
                            ViewBag.MemberProfileUrl = "";
                        }
                        else
                        {
                            ViewBag.UserPass = MemberUserIDPassword.U_Pass;

                            ViewBag.MemberDetails = ChurchMember;
                            ViewBag.UserId = MemberUserIDPassword.U_ID;

                            var BasUrl = (from data in dbcontext.DocumentBaseUrls select data).FirstOrDefault();
                            if (BasUrl != null)
                            {
                                ViewBag.MemberProfileUrl = BasUrl.BaseUrl;
                            }
                        }
                    }
                    else
                    {
                        var ChurchMember = (from data in dbcontext.MAS_INDVSL where data.FID == MemberFId && data.MAS_CHC_FID == MAS_CHC_FID && data.Deactivate == false select data).FirstOrDefault();

                        var MemberUserIDPassword = (from data1 in dbcontext.MAS_UID where data1.MAS_INDVSL_FID == MemberFId && data1.MAS_CHC_FID == MAS_CHC_FID && data1.U_Status == true select data1).FirstOrDefault();


                        if (MemberUserIDPassword == null)
                        {
                            ViewBag.UserId = "";
                            ViewBag.UserPass = "";

                            ViewBag.MemberDetails = "";
                            ViewBag.MemberProfileUrl = "";
                        }
                        else
                        {
                            ViewBag.UserPass = MemberUserIDPassword.U_Pass;

                            ViewBag.MemberDetails = ChurchMember;
                            ViewBag.UserId = MemberUserIDPassword.U_ID;

                            var BasUrl = (from data in dbcontext.DocumentBaseUrls select data).FirstOrDefault();
                            if (BasUrl != null)
                            {
                                ViewBag.MemberProfileUrl = BasUrl.BaseUrl;
                            }
                        }
                    }

                }
                else
                {

                    if (Session["LeaderFId"] == null)
                    {
                        return RedirectToAction("Login", "Home", new { area = "" });
                    }
                    var LeaderFId = Session["LeaderFId"];
                    var LeaderChurchId = Session["LeaderCurchId"];
                    int LeaderChurchFId = Convert.ToInt32(LeaderChurchId);
                    int LeaderFid = Convert.ToInt32(LeaderFId);

                    var MemberPrayerRequest = (from data in dbcontext.MAS_PrayerReq where data.Deactivate == false && data.Req_ID == MemberFId && data.Req_Status == false && data.MAS_CHC_FID == LeaderChurchFId select data).ToList();
                    ViewBag.MemberPendingPrayerRequest = MemberPrayerRequest.Count();

                    //var MemberEnquiryRequest = (from data in dbcontext.Mas_Enquiry where data.Status == true && data.MemberFId == MemberFId && data.LeaderResponseStatus == true && data.CurchId == LeaderChurchFId select data).ToList();
                    //var MemberEnquiryRequest = dbcontext.sp_List_Enquiry(null, null, LeaderFid,null).ToList();
                    var MemberEnquiryRequest = (from data in dbcontext.Mas_Enquiry
                                                where data.Status == true
                                                      && data.MemberFId == MemberFId
                                                      && data.CurchId == MAS_CHC_FID
                                                      && ((data.Status == true && data.LeaderResponseStatus == true)
                                                          || (data.Status == false && data.LeaderResponseStatus == true))
                                                select data).ToList();
                    ViewBag.MemberPendingEnquiryRequest = MemberEnquiryRequest.Count();

                    var DonationAmount = (from data in dbcontext.Mas_Donation where data.MemberFId == MemberFId && data.ChurchFId == LeaderChurchFId && data.Status == true && data.Deactivate == false select data).ToList();
                    ViewBag.Donation = DonationAmount.Count();


                    if (MAS_CHC_FID == null)
                    {
                        var ChurchMember = (from data in dbcontext.MAS_INDVSL where data.FID == MemberFId && data.MAS_CHC_FID == LeaderChurchFId && data.Deactivate == false select data).FirstOrDefault();


                        var MemberUserIDPassword = (from data1 in dbcontext.MAS_UID where data1.MAS_INDVSL_FID == MemberFId && data1.MAS_CHC_FID == LeaderChurchFId && data1.U_Status == true select data1).FirstOrDefault();


                        if (MemberUserIDPassword == null)
                        {
                            ViewBag.UserId = "";
                            ViewBag.UserPass = "";

                            ViewBag.MemberDetails = "";
                            ViewBag.MemberProfileUrl = "";
                        }
                        else
                        {
                            ViewBag.UserPass = MemberUserIDPassword.U_Pass;

                            ViewBag.MemberDetails = ChurchMember;
                            ViewBag.UserId = MemberUserIDPassword.U_ID;

                            var BasUrl = (from data in dbcontext.DocumentBaseUrls select data).FirstOrDefault();
                            if (BasUrl != null)
                            {
                                ViewBag.MemberProfileUrl = BasUrl.BaseUrl;
                            }
                        }
                    }
                    else
                    {
                        var ChurchMember = (from data in dbcontext.MAS_INDVSL where data.FID == MemberFId && data.MAS_CHC_FID == MAS_CHC_FID && data.Deactivate == false select data).FirstOrDefault();

                        var MemberUserIDPassword = (from data1 in dbcontext.MAS_UID where data1.MAS_INDVSL_FID == MemberFId && data1.MAS_CHC_FID == MAS_CHC_FID && data1.U_Status == true select data1).FirstOrDefault();


                        if (MemberUserIDPassword == null)
                        {
                            ViewBag.UserId = "";
                            ViewBag.UserPass = "";

                            ViewBag.MemberDetails = "";
                            ViewBag.MemberProfileUrl = "";
                        }
                        else
                        {
                            ViewBag.UserPass = MemberUserIDPassword.U_Pass;

                            ViewBag.MemberDetails = ChurchMember;
                            ViewBag.UserId = MemberUserIDPassword.U_ID;

                            var BasUrl = (from data in dbcontext.DocumentBaseUrls select data).FirstOrDefault();
                            if (BasUrl != null)
                            {
                                ViewBag.MemberProfileUrl = BasUrl.BaseUrl;
                            }
                        }
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


        public ActionResult UpdateMemberDetails(string pk, string value, int memberId)
        {
            try
            {

                var UpdateData = (from data in dbcontext.MAS_INDVSL where data.Deactivate == false && data.FID == memberId select data).FirstOrDefault();

                if(pk== "UserName")
                {
                    UpdateData.IND_Name = value;
                }
                else if(pk == "contact")
                {
                    UpdateData.IND_Mob = value;
                }
                else if(pk == "EmailId")
                {
                    UpdateData.IND_Email = value;
                }
                else if (pk == "DateOfBirth")
                {
                    DateTime date = Convert.ToDateTime(value);
                    UpdateData.IND_DOB = date;
                }
                else if (pk == "Gender")
                {
                    UpdateData.Gender = value;
                }
                else if (pk == "Address")
                {
                    UpdateData.IND_Address = value;
                }
                int i = dbcontext.SaveChanges();
                if (i >= 0)
                {                   
                    return RedirectToAction("ChurchMemberDetails", "ChurchMemberDetails", new { area = "Leader",MemberFId = memberId });
                }else
                {
                    return RedirectToAction("ChurchMemberDetails", "ChurchMemberDetails", new { area = "Leader", MemberFId = memberId });
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