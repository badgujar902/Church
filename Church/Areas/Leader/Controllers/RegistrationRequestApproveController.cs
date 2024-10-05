using Church.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Church.Areas.Leader.Controllers
{
    public class RegistrationRequestApproveController : Controller
    {
        CHC_NewEntities dbcontext = new CHC_NewEntities();
        DateTime TodaysDate = DateTime.Now;
        string LoginMachinId = Dns.GetHostName();
        // GET: Leader/RegistrationRequestApprove
        public ActionResult RegistrationRequestApprove(int? id)
        {
            try
            {
                if (Session["HeadLeader_U_Fid"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }

                MAS_INDVSL PendingRegistartionMember = new MAS_INDVSL();

                PendingRegistartionMember = (from data in dbcontext.MAS_INDVSL where data.FID == id select data).FirstOrDefault();

                ViewBag.MemberMobileNo = PendingRegistartionMember.IND_Mob;


                return View(PendingRegistartionMember);
            }
            catch (Exception ex)
            {
                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
                throw;
            }

        }
        [HttpGet]
        public ActionResult SaveRegistrationRequestApprove(int? INDVSLFID, string UserName, string UserPassword)
        {
            try
            {
                if (Session["HeadLeader_U_Fid"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
                if (INDVSLFID != null)
                {
                    var Update = (from data in dbcontext.MAS_INDVSL where data.FID == INDVSLFID select data).FirstOrDefault();
                    Update.Status = false;
                    Update.IND_DOJ = DateTime.Now;
                    int update = dbcontext.SaveChanges();
                    var LeaderFId = Session["HeadLeader_U_Fid"];
                    var CurchFID = Session["HeadLeaderCurchId"];
                    string LoginMachinIP = Dns.GetHostByName(LoginMachinId).AddressList[0].ToString();

                    var GetchurchName = (from data in dbcontext.MAS_CHC where data.FID == Update.MAS_CHC_FID && data.Status == true select data).FirstOrDefault();

                    MAS_UID MasUserid = new MAS_UID();
                    if (update > 0)
                    {
                        MasUserid.FDate = DateTime.Now;
                        MasUserid.MacID = LoginMachinId;
                        MasUserid.MacIP = LoginMachinIP;
                        MasUserid.MAS_INDVSL_FID = INDVSLFID;
                        MasUserid.U_Type = "Individual";
                        MasUserid.U_ID = UserName;
                        MasUserid.U_Pass = UserPassword;
                        MasUserid.U_Status = true;
                        MasUserid.MAS_LEADER_FID = Convert.ToInt32(LeaderFId);
                        MasUserid.MAS_CHC_FID = Convert.ToInt32(CurchFID);
                        dbcontext.MAS_UID.Add(MasUserid);
                        int i = dbcontext.SaveChanges();

                        SendMail mail = new SendMail();
                        var Title = "User ID and Password";
                        var GetBody = "Dear <b>" + Update.IND_Name + "</b>,<br><br>" +
                                         "Your default username and password are as follows:<br>" +
                                         "Username: <b>" + UserName + "</b><br>" +
                                         "Password: <b>" + UserPassword + "</b><br><br>" +
                                         "To reset your username and password, click the link below:<br>" +
                                         "<a href='http://ekstasisministries.org/'>Reset Here</a><br><br>" +
                                         "Best regards,<br>" +
                                         GetchurchName.CHC_Name;
                        mail.SendMailToMember(Update.IND_Email, GetBody, Title);
                        if (i != 0)
                        {
                            //var body = "Dear <b>" + Update.IND_Name + "</b>,<br><br> Your Deafult UserName and Password <br> UserName : <b>" + UserName + "</b><br> Password : <b> " + UserPassword + " <br><br><br> Reset Your UserName and Password <br>Click here  <b> Best regards <br><br> " + GetchurchName.CHC_Name + "";

                            TempData["Message"] = "Member  Created Successfully";
                            TempData["Icon"] = "success";
                            return Json(new { Message = TempData["Message"], Icon = TempData["Icon"] }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            TempData["Message"] = "Member  Not Created Successfully";
                            TempData["Icon"] = "error";
                            return Json(new { Message = TempData["Message"], Icon = TempData["Icon"] }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        TempData["Message"] = "Member  Not Created Successfully";
                        TempData["Icon"] = "error";
                        return Json(new { Message = TempData["Message"], Icon = TempData["Icon"] }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    TempData["Message"] = "Member  Not Created Successfully";
                    TempData["Icon"] = "error";
                    return Json(new { Message = TempData["Message"], Icon = TempData["Icon"] }, JsonRequestBehavior.AllowGet);
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