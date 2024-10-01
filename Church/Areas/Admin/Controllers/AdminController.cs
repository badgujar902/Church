using Church.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Church.Models;
using System.IO;
using Church.Areas.Admin.Models;
using System.Data.Entity;

namespace Church.Areas.Admin.Controllers
{
    public class AdminController : Controller
    {

        CHC_NewEntities dbcontext = new CHC_NewEntities();
        DateTime TodaysDate = DateTime.Now;
        string LoginMachinId = Dns.GetHostName();
        //   string LoginmachinIp = Dns.GetHostByName(LoginMachinId).AddressList[0].ToString();
        // GET: Admin/Admin
        public ActionResult Dashboard()
        {
            try
            {
                if (Session["Admin"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
                var PrayerRequest = dbcontext.PendingPrayerRequest(null,null,null,null).ToList();
                TempData["PrayerRequestCount"] = PrayerRequest.Count();

                var GetResitration = dbcontext.MAS_INDVSL.Where(x => x.Status == true  && x.Enroll_Type == "Self" && x.Deactivate == false).ToList();
                TempData["GetResitrationRequestCount"] = GetResitration.Count();

                var GetTotalMemberList = dbcontext.sp_List_CurchMember(null,null,null,null).ToList();
                TempData["TotalMeberList"] = GetTotalMemberList.Count();


                var query = from enquiry in dbcontext.Mas_Enquiry
                            join member in dbcontext.MAS_INDVSL
                                on enquiry.MemberFId equals member.FID
                            join church in dbcontext.MAS_CHC
                                on enquiry.CurchId equals church.FID
                            where enquiry.Deactivate == false                             
                                && ((enquiry.Status == true && enquiry.LeaderResponseStatus == true)
                                    || (enquiry.Status == false && enquiry.LeaderResponseStatus == true))
                            orderby enquiry.FId descending
                            select new
                            {
                                enquiry.Subject,
                                enquiry.Enquiry,
                                enquiry.MemberFId,                               
                                MemberName = member.IND_Name,
                                MemberEmail = member.IND_Email,
                                ChurchName = church.CHC_Name,
                                ChurchFId = church.FID
                            };

                var Enquiry = query.ToList();

                //var GetEnquiry = dbcontext.sp_List_Enquiry(null,null,null).ToList();
                //TempData["TotalEnquiryList"] = GetEnquiry.Count();
                TempData["TotalEnquiryList"] = Enquiry.Count();
                return View();
            }
            catch (Exception ex)
            {

                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }
          
        }

        #region List
        public ActionResult GetChurchList()
        {
            try
            {
                if (Session["UserName"] != null)
                {

                    var church = dbcontext.MAS_CHC.Where(X => X.Status == true).ToList();
                    ViewBag.ChurchList = church;
                    return View();
                }
                else
                {

                    return RedirectToAction("Login", "Home", new { area = "" });
                }
            }
            catch (Exception Ex)
            {
                return RedirectToAction("Login", "Home", new { area = "" });
            }
        }
        #endregion List

        #region ChurchCreation
        public ActionResult ChurchCreation()
        {
            try
            {
                if (Session["Admin"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
                var GetState = (from data in dbcontext.MAS_STATE where data.Active == 1 select new BindDrop { Id = data.Fid, Name = data.State }).ToList();
                ViewBag.GEtStateName = GetState;

                var church = dbcontext.MAS_CHC.Where(X => X.Status == true).ToList();
                ViewBag.ChurchList = church;
                return View();
            }
            catch (Exception ex)
            {
                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }
        }
        [HttpPost]
        public ActionResult SaveChurchCreation(ChurchCreate church)
        {
         
            string LoginmachinIp = Dns.GetHostByName(LoginMachinId).AddressList[0].ToString();
            try
            {
                if (Session["Admin"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
                MAS_CHC mas_church = new MAS_CHC();
                int save = 0;
             
                    if (church.Mas_City != null)
                    {
                        var PostCodeFid = (from data in dbcontext.MAS_POSTCOD where data.State_Fid == church.Mas_StateId && data.City == church.Mas_City && data.Area == church.Area select data).FirstOrDefault();

                        if (church != null)
                        {
                            mas_church.FDate = TodaysDate;
                            mas_church.MacID = LoginMachinId;
                            mas_church.MacIP = LoginmachinIp;
                            mas_church.CHC_Name = church.ChurchName;
                            mas_church.Mas_State_Fid = church.Mas_StateId;
                            if (PostCodeFid != null)
                            {
                                mas_church.Mas_Postcode_Fid = PostCodeFid.Fid;
                            }
                            mas_church.CHC_Address = church.ChurchAddress;
                            mas_church.Status = true;

                            dbcontext.MAS_CHC.Add(mas_church);
                            save = dbcontext.SaveChanges();

                        }
                    }            
              
                if(save!=0)
                {
                    TempData["Message"] = "Church Created Successfully";
                    TempData["Icon"] = "success";
                    return RedirectToAction("ChurchCreation", "Admin");
                }
                else
                {
                    TempData["Message"] = "Church Not Created ";
                    TempData["Icon"] = "error";
                    return RedirectToAction("ChurchCreation", "Admin");
                }             
            }
            catch (Exception ex)
            {
                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });

            }
        }

   
        #endregion Church

    



        #region GetCity
        public ActionResult GetCity(int? StateId)
        {
            try
            {
                if (Session["Admin"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }

                var Data = (from data in dbcontext.MAS_POSTCOD
                            where data.State_Fid == StateId
                            select new BindDrop { Name = data.City })
                    .Distinct()
                    .ToList();
                return Json(Data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }
        }
        #endregion

    

        #region GetArea
        public ActionResult GetArea(int? StateId, string MasCityName)
        {
            try
            {
                if (Session["Admin"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
                var Data = (from data in dbcontext.MAS_POSTCOD
                            where data.State_Fid == StateId && data.City == MasCityName
                            select new BindDrop { Name = data.Area })
                             .Distinct()
                            .ToList();

                //var Data = dbcontext.MAS_POSTCOD
                //  .Where(data => data.State_Fid == StateId && data.City == MasCityName)
                //  .GroupBy(data => data.Area)
                //  .Select(group => new BindDrop { Id = group.First().Fid, Name = group.Key })
                //  .ToList();


                return Json(Data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }
        }
        #endregion


        #region PostCode
        public ActionResult PostCode(int? StateId, string CityArea)
        {
            try
            {
                if (Session["Admin"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
                var Data = (from data in dbcontext.MAS_POSTCOD
                            where data.State_Fid == StateId && data.Area == CityArea
                            select new BindDrop { Name = data.PostalCode })
                             .Distinct()
                            .ToList();

                return Json(Data, JsonRequestBehavior.AllowGet);
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