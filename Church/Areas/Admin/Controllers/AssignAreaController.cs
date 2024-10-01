using Church.Areas.Admin.Models;
using Church.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Church.Areas.Admin.Controllers
{
    public class AssignAreaController : Controller
    {
        CHC_NewEntities dbcontext = new CHC_NewEntities();
        DateTime TodaysDate = DateTime.Now;
        string LoginMachinId = Dns.GetHostName();
        // GET: Admin/AssignArea
        public ActionResult AssignArea(int? FID,int? MAS_CHC_FID)
        {
            try
            {
                if (Session["Admin"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
                var GetState = (from data in dbcontext.MAS_STATE where data.Active == 1 select new BindDrop { Id = data.Fid, Name = data.State }).ToList();
                ViewBag.GEtStateName = GetState;

                var GetLeader = (from data in dbcontext.MAS_LEADER where data.FID == FID && data.MAS_CHC_FID == MAS_CHC_FID select new BindDrop { Id = data.FID, Name = data.Lead_Name }).ToList();
                ViewBag.GetLeaderName = GetLeader;

                var GetList = dbcontext.sp_List_AssignArea(FID, MAS_CHC_FID).ToList();
                ViewBag.GetAssignList = GetList;

                return View();
            }
            catch (Exception ex)
            {
                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }
      
        }

        //[HttpPost]
        //public ActionResult IsExistsArea(int? LeaderFid, List<string> Area)
        //{
        //    try
        //    {
        //        var GetChurch = (from data in dbcontext.Mas_AssignArea
        //                         where Area.Contains((string)data.Area) && data.Mas_LeaderFid == LeaderFid
        //                         select new BindDrop { Name = data.Area }).ToList();

        //        return View();
        //    }
        //    catch (Exception ex)
        //    {

        //        @Session["GetErrorMessage"] = ex.Message;
        //        return RedirectToAction("Error", "Home", new { area = "" });
        //    }
        //}
        [HttpPost]
        public ActionResult IsExistsArea(int LeaderFid, List<string> Area)
        {
            try
            {
                //var GetArea = (from data in dbcontext.Mas_AssignArea
                //                 where Area.Contains(data.Area) && data.Mas_LeaderFid == LeaderFid && data.Deleted==false
                //                 select  data.Area ).ToList();

                var GetArea = (from data in dbcontext.Mas_AssignArea
                               where Area.Contains(data.Area) && data.Deleted == false
                               select data.Area).ToList();

                var AreaAssignLeaderFid = (from data in dbcontext.Mas_AssignArea
                                           where Area.Contains(data.Area) && data.Deleted == false
                                           select data.Mas_LeaderFid).ToList();
                var GetLeaderName = (from data in dbcontext.MAS_LEADER where AreaAssignLeaderFid.Contains(data.FID) && data.Status == true select data.Lead_Name).ToList();


                if (GetArea.Count!=0)
                {
                    TempData["Message"] = "Area Already Assigned to Leader";
                    TempData["Icon"] = "error";
                    TempData["Area"] = string.Join(",", GetArea) + " Area Already Assigned to " + string.Join(",", GetLeaderName) + " Leader";
                    return Json(new { Message = TempData["Message"], Icon = TempData["Icon"], Area = TempData["Area"] });
                }
                else
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }

           

                // Return a JSON result (or modify this based on your needs)
               
            }
            catch (Exception ex)
            {
                Session["GetErrorMessage"] = ex.Message;
                return Json(new { Message = "Error occurred", Icon = "error" });
            }
        }

        [HttpPost]
        public ActionResult SaveAssignArea(Mas_AssignArea assignArea,List<string> Area)
        {
            try
            {
                if (Session["Admin"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
                int save = 0;               
                var LoginmachinIp = Dns.GetHostByName(LoginMachinId).AddressList[0].ToString();
                int LeaderFid = Convert.ToInt32(assignArea.Mas_LeaderFid);

                var getLeaderChurchFid = (from data in dbcontext.MAS_LEADER where data.FID == LeaderFid select new { data.MAS_CHC_FID }).FirstOrDefault();

                if(assignArea!=null)
                {

                    int count =Area.Count;
                    for(int i=0;i< count;i++)
                    {
                        Mas_AssignArea ASSArea = new Mas_AssignArea();
                        ASSArea.FDate = DateTime.Now;
                        ASSArea.MacID = LoginMachinId;
                        ASSArea.MacIP = LoginmachinIp;
                        ASSArea.Mas_LeaderFid = assignArea.Mas_LeaderFid;
                        if (getLeaderChurchFid != null)
                        {
                            ASSArea.Mas_churchFId = getLeaderChurchFid.MAS_CHC_FID;
                        }
                        ASSArea.StateFid = assignArea.StateFid;
                        ASSArea.City = assignArea.City;
                        //ASSArea.Area = string.Join(",", Area);
                        ASSArea.Area =  Area[i];
                        ASSArea.Deleted = false;
                        dbcontext.Mas_AssignArea.Add(ASSArea);
                    }
                   
                    save = dbcontext.SaveChanges();

                }
                if (save != 0)
                {
                    TempData["Message"] = "Area Assign Successfully";
                    TempData["Icon"] = "success";
                    return RedirectToAction("LeaderCreateList", "LeaderCreateList", new { area = "Admin" });
                }
                else
                {
                    TempData["Message"] = "Area not Assign";
                    TempData["Icon"] = "error";
                    return RedirectToAction("LeaderCreateList", "LeaderCreateList", new { area = "Admin" });
                }
            
            }
            catch (Exception ex)
            {

                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }
        }



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

        //public ActionResult AssignAreaList()
        //{
        //    try
        //    {

        //        if (Session["Admin"] == null)
        //        {
        //            return RedirectToAction("Login", "Home", new { area = "" });
        //        }
        //        //var GetList = (from data in dbcontext.Mas_AssignArea where data.Deleted == false select data).ToList();
            
        //        return View();
        //    }
        //    catch (Exception ex)
        //    {

        //        @Session["GetErrorMessage"] = ex.Message;
        //        return RedirectToAction("Error", "Home", new { area = "" });
        //    }
        //}

        public ActionResult UpdateAssignArea(int? mas_LeaderFid, int? mas_churchFID)
        {
            try
            {
                if (Session["Admin"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
                Mas_AssignArea ASSArea = new Mas_AssignArea();
                ASSArea = (from data in dbcontext.Mas_AssignArea where data.Mas_LeaderFid == mas_LeaderFid && data.Mas_churchFId == mas_churchFID select data).FirstOrDefault();
                

                var GetState = (from data in dbcontext.MAS_STATE where data.Active == 1 select new BindDrop { Id = data.Fid, Name = data.State }).ToList();
                ViewBag.GEtStateName = GetState;

                var GetLeader = (from data in dbcontext.MAS_LEADER where data.FID == mas_LeaderFid && data.MAS_CHC_FID == mas_churchFID select new BindDrop { Id = data.FID, Name = data.Lead_Name }).ToList();
                ViewBag.GetLeaderName = GetLeader;

                var GetCity = (from data in dbcontext.MAS_POSTCOD where data.State_Fid == ASSArea.StateFid select new BindDrop { Name = data.City }).ToList();
                ViewBag.GetCityName = GetCity;

                var GetArea = (from data in dbcontext.MAS_POSTCOD where data.State_Fid == ASSArea.StateFid && data.City== ASSArea.City select new BindDrop { Name = data.Area }).ToList();
                ViewBag.GetAreaName = GetArea;
                return View(ASSArea);
            }
            catch (Exception ex)
            {

                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }
        }
        [HttpPost]
        public ActionResult SaveUpdateAssignArea(Mas_AssignArea assignArea, List<string> Area)
        {
            try
            {
                int update = 0;
                var UpdateArea = (from data in dbcontext.Mas_AssignArea where data.Mas_LeaderFid == assignArea.Mas_LeaderFid && data.Mas_churchFId == assignArea.Mas_churchFId && data.Deleted == false select data).FirstOrDefault();
                UpdateArea.StateFid = assignArea.StateFid;
                UpdateArea.City = assignArea.City;
                UpdateArea.Mas_LeaderFid = assignArea.Mas_LeaderFid;
                UpdateArea.Area= string.Join(",", Area);
                update = dbcontext.SaveChanges();

                if (update >= 0)
                {
                    TempData["Message"] = "Area Updated Successfully";
                    TempData["Icon"] = "success";
                    return RedirectToAction("AssignAreaList", "AssignArea", new { area = "Admin" });
                }
                else
                {
                    TempData["Message"] = "Area not Update";
                    TempData["Icon"] = "error";
                    return RedirectToAction("AssignAreaList", "AssignArea", new { area = "Admin" });
                }


            }
            catch (Exception ex)
            {
                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }
        }

        public ActionResult DeleteArea(int? mas_LeaderFid,int? mas_churchFID)
        {
            try
            {
                int Delete = 0;
                var UpdateArea = (from data in dbcontext.Mas_AssignArea where data.Mas_LeaderFid == mas_LeaderFid && data.Mas_churchFId == mas_churchFID && data.Deleted == false select data).FirstOrDefault();
                UpdateArea.Deleted = true;
                Delete = dbcontext.SaveChanges();
                if (Delete != 0)
                {
                    TempData["Message"] = "Area Deleted Successfully";
                    TempData["Icon"] = "success";
                    return RedirectToAction("AssignAreaList", "AssignArea", new { area = "Admin" });
                }
                else
                {
                    TempData["Message"] = "Area not Delete";
                    TempData["Icon"] = "error";
                    return RedirectToAction("AssignAreaList", "AssignArea", new { area = "Admin" });
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