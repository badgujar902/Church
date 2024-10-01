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
    public class ChurchCreationListController : Controller
    {

        CHC_NewEntities dbcontext = new CHC_NewEntities();
        DateTime TodaysDate = DateTime.Now;
        string LoginMachinId = Dns.GetHostName();
        // GET: Admin/ChurchCreationList
        public ActionResult ChurchCreationList()
        {
            try
            {
                if (Session["Admin"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }

                //var ChurchList=(from data in dbcontext.MAS_CHC where data.Status==true)
                //var churchList = (from church in dbcontext.MAS_CHC
                //                  join state in dbcontext.MAS_STATE
                //                  on church.Mas_State_Fid equals state.Fid
                //                  join cityArea in dbcontext.MAS_POSTCOD
                //                  on church.Mas_Postcode_Fid equals cityArea.Fid
                //                  where church.Status == true
                //                  select new
                //                  {
                //                      ChurchName = church.CHC_Name,
                //                      churchFDate = church.FDate,
                //                      StateName = state.State,
                //                      StateFid = state.Fid, // Adjust field names as needed
                //                      CityName = cityArea.City,
                //                      AreaName = cityArea.Area,
                //                      PostCodeFid = cityArea.Fid,
                //                      ChurchId = church.FID
                //                      // Include other fields you need
                //                  }).ToList();

                //ViewBag.ChurchCreateList = churchList;

                var churchList = dbcontext.sp_List_Church().ToList();
                ViewBag.ChurchCreateList = churchList;
            }
            catch (Exception ex)
            {
                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }
         
            return View();
        }

        public ActionResult UpdateChurch(int? churchFid,int? postcodeFid)
        {
            try
            {
                if (Session["Admin"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
                var GetState = (from data in dbcontext.MAS_STATE where data.Active == 1 select new BindDrop { Id = data.Fid, Name = data.State }).ToList();
                ViewBag.GEtStateName = GetState;

                MAS_CHC church = new MAS_CHC();
                church = (from data in dbcontext.MAS_CHC where data.FID == churchFid && data.Mas_Postcode_Fid == postcodeFid select data).FirstOrDefault();

                return View(church);
            }
            catch (Exception ex)
            {
                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }
        }

        [HttpPost]
        public ActionResult SaveUpdateChurch(MAS_CHC church,int? State,string City,string Area)
        {
            try
            {
                if (Session["Admin"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
                int update = 0;
                if(State!=null && City!=null)
                {
                    var postCodeFid = (from data in dbcontext.MAS_POSTCOD where data.State_Fid == State && data.City == City && data.Area == Area select data).FirstOrDefault();
                    if (church != null)
                    {
                        var UpdateChurch = (from data in dbcontext.MAS_CHC where data.FID == church.FID && data.Status == true select data).FirstOrDefault();

                        UpdateChurch.CHC_Name = church.CHC_Name;
                        UpdateChurch.CHC_Address = church.CHC_Address;
                        if(postCodeFid!=null)
                        {
                            UpdateChurch.Mas_Postcode_Fid = postCodeFid.Fid;
                        }

                        update = dbcontext.SaveChanges();
                    }
                }
                else
                {
                    if(church!=null)
                    {
                        var UpdateChurch = (from data in dbcontext.MAS_CHC where data.FID == church.FID && data.Status == true select data).FirstOrDefault();

                        UpdateChurch.CHC_Name = church.CHC_Name;
                        UpdateChurch.CHC_Address = church.CHC_Address;

                        update = dbcontext.SaveChanges();

                    }
                }
                if(update>=0)
                {
                    TempData["Message"] = "Church Updated successfully";
                    TempData["Icon"] = "success";
                    return RedirectToAction("ChurchCreationList", "ChurchCreationList",new {area= "Admin" });
                }else
                {
                    TempData["Message"] = "Church not Updated successfully";
                    TempData["Icon"] = "error";
                    return RedirectToAction("ChurchCreationList", "ChurchCreationList", new { area = "Admin" });
                }
             
            }
            catch (Exception ex)
            {
                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }
        }

        public ActionResult DeleteChurch(int? churchFid, int? postcodeFid)
        {
            try
            {
                if (Session["Admin"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
                int Delete = 0;
                var DeleteChurch = (from data in dbcontext.MAS_CHC where data.FID == churchFid && data.Mas_Postcode_Fid == postcodeFid select data).FirstOrDefault();

                DeleteChurch.Status = false;
                Delete = dbcontext.SaveChanges();
                if(Delete>=0)
                {
                    TempData["Message"] = "Church Deleted successfully";
                    TempData["Icon"] = "success";
                    return RedirectToAction("ChurchCreationList", "ChurchCreationList", new { area = "Admin" });
                }else
                {
                    TempData["Message"] = "Church not Deleted successfully";
                    TempData["Icon"] = "error";
                    return RedirectToAction("ChurchCreationList", "ChurchCreationList", new { area = "Admin" });
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