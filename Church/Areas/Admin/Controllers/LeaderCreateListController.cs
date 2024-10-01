using Church.Areas.Admin.Models;
using Church.Models;
using Newtonsoft.Json;
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
    public class LeaderCreateListController : Controller
    {
        CHC_NewEntities dbcontext = new CHC_NewEntities();
        DateTime TodaysDate = DateTime.Now;
        string LoginMachinId = Dns.GetHostName();
        // GET: Admin/LeaderCreateList
        #region LeaderCreateList
        public ActionResult LeaderCreateList()
        {
            try
            {
                if (Session["Admin"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }

                var LeaderList = dbcontext.sp_List_LeaderCreate().ToList();
                ViewBag.LeaderCreateList = LeaderList;

                return View();
            }
            catch (Exception ex)
            {
                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }
        }
        #endregion


        #region UpdateLeader
        public ActionResult UpdateLeader(int? FID, int? MAS_CHC_FID)
        {
            try
            {
                if (Session["Admin"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
                MAS_LEADER Leader = new MAS_LEADER();

                Leader = (from data in dbcontext.MAS_LEADER where data.FID == FID && data.MAS_CHC_FID == MAS_CHC_FID && data.Status == true select data).FirstOrDefault();
                string imageNameWithExtension = Path.GetFileName(Leader.Image);
                ViewBag.file = "Uploaded Image " + imageNameWithExtension;

                Session["ChurchFid"] = Leader.MAS_CHC_FID;



                var GetLeaderPosition = (from data in dbcontext.MAS_UID where data.MAS_LEADER_FID == FID && data.U_Status == true select data).FirstOrDefault();
                if (GetLeaderPosition.U_Type == "Head Leader")
                {
                    ViewBag.LeaderPosition = "Co-Worker";
                }
                else
                {
                    ViewBag.LeaderPosition = "Head Pastor";
                }


                var GetLeaderType = (from data in dbcontext.MAS_UID where data.MAS_LEADER_FID == Leader.FID && data.U_Status == true select data).FirstOrDefault();
                ViewBag.LeaderType = GetLeaderType.U_Type;

                Session["LeaderPosition"] = GetLeaderType.U_Type; ;

                var GetChurch = (from data in dbcontext.MAS_CHC where data.Status == true select data).ToList();
                ViewBag.GetChurchList = GetChurch;

                var GetState = (from data in dbcontext.MAS_STATE where data.Active == 1 select new BindDrop { Id = data.Fid, Name = data.State }).ToList();
                ViewBag.GEtStateName = GetState;

                //var GetCity =(from data in dbcontext.MAS_POSTCOD where data.State_Fid== Leader.StateFid)
                var GetCity = (from data in dbcontext.MAS_POSTCOD
                               where data.State_Fid == Leader.StateFid
                               select new BindDrop { Name = data.City })
             .Distinct()
             .ToList();

                ViewBag.GetLeaderCity = GetCity;

                var GetLeaderAssignArea = (from data in dbcontext.Mas_AssignArea where data.Mas_LeaderFid == FID && data.Mas_churchFId == MAS_CHC_FID && data.Deleted == false select data).ToList();
                ViewBag.GetAssignArea = GetLeaderAssignArea;


                var GetCityState = (from data in dbcontext.MAS_LEADER where data.FID == FID && data.MAS_CHC_FID == MAS_CHC_FID select data).FirstOrDefault();
                if (GetCityState != null)
                {
                    var Data = (from data in dbcontext.MAS_POSTCOD
                                where data.State_Fid == GetCityState.StateFid && data.City == GetCityState.CityName
                                select new BindDrop { Name = data.Area })
                                         .Distinct()
                                        .ToList();

                    ViewBag.GetArea = Data;
                }


                return View(Leader);
            }
            catch (Exception ex)
            {
                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }
        }

        [HttpPost]
        public ActionResult IsExistsData(int? LeaderFId, string Email, string MobileNumber, List<string> Area)
        {
            try
            {
                int error = 0;
                string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
                if (Area != null)
                {
                    var GetArea = (from data in dbcontext.Mas_AssignArea
                                   where Area.Contains(data.Area) && data.Deleted == false
                                   select data.Area).ToList();

                    var AreaAssignLeaderFid = (from data in dbcontext.Mas_AssignArea
                                               where Area.Contains(data.Area) && data.Deleted == false
                                               select data.Mas_LeaderFid).ToList();
                    var GetLeaderName = (from data in dbcontext.MAS_LEADER where AreaAssignLeaderFid.Contains(data.FID) && data.Status == true select data.Lead_Name).ToList();

                    if (GetArea.Count != 0)
                    {
                        TempData["Message"] = "Area Already Assigned to Leader";
                        TempData["Icon"] = "error";
                        TempData["Area"] = string.Join(",", GetArea) + " Area Already Assigned to " + string.Join(",", GetLeaderName) + " Leader";
                        return Json(new { Message = TempData["Message"], Icon = TempData["Icon"], Area = TempData["Area"] });
                        //return Json(new { Message = TempData["Message"], Icon = TempData["Icon"], Area = string.Join(",", GetArea) + string.Join(",", GetLeaderName) });
                    }

                }
                else if (Email != "")
                {
                    if (!Regex.IsMatch(Email, pattern))
                    {
                        error = 1;
                        TempData["Message"] = "Enter Valid Email Address !";
                        TempData["Icon"] = "error";
                        return Json(new { Message = TempData["Message"], Icon = TempData["Icon"] }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                if (error != 0)
                {
                    return Json(new { Message = TempData["Message"], Icon = TempData["Icon"] }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }


            }
            catch (Exception ex)
            {
                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }
        }

        [HttpPost]
        public ActionResult SaveUpdateLeader(MAS_LEADER Leader, HttpPostedFileBase file, string Postion, string tableData, List<string> Area)
        {
            try
            {
                if (Session["Admin"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
                var ChurchID = Session["ChurchFid"];
                int ChurchFid = Convert.ToInt32(ChurchID);
                int save = 0;
                var LoginmachinIp = Dns.GetHostByName(LoginMachinId).AddressList[0].ToString();


                HttpPostedFile files = System.Web.HttpContext.Current.Request.Files["file"];
                var update = (from data in dbcontext.MAS_LEADER where data.FID == Leader.FID && data.Status == true select data).FirstOrDefault();

                update.Lead_Name = Leader.Lead_Name;
                update.Lead_Mob = Leader.Lead_Mob;
                update.Lead_EmailID = Leader.Lead_EmailID;
                update.Lead_DOB = Leader.Lead_DOB;
                update.Gender = Leader.Gender;
                update.Lead_Add = Leader.Lead_Add;
                if (Leader.StateFid != null)
                {
                    update.StateFid = Leader.StateFid;
                }
                if (Leader.CityName != null)
                {
                    update.CityName = Leader.CityName;
                }
                if (Leader.MAS_CHC_FID != null)
                {
                    update.MAS_CHC_FID = Leader.MAS_CHC_FID;
                    var UpdateMas_Uid = (from data in dbcontext.MAS_UID where data.MAS_LEADER_FID == Leader.FID && (data.U_Type == "Head Leader" || data.U_Type == "Leader") && data.U_Status == true select data).FirstOrDefault();
                    if (ChurchFid != Leader.MAS_CHC_FID)
                    {
                        UpdateMas_Uid.MAS_CHC_FID = Leader.MAS_CHC_FID;
                    }
                    if (Session["LeaderPosition"].ToString() != Postion)
                    {
                        UpdateMas_Uid.U_Type = Postion;
                    }
                    dbcontext.SaveChanges();
                }

                var filelctn = Leader.Lead_Mob;
                string MoveLocation = @"C:\StealthChurch\RegistrationData\\" + filelctn + "\\";
                if (files != null)
                {
                    if (files.ContentLength > 0)
                    {
                        if (!Directory.Exists(MoveLocation))
                        {
                            Directory.CreateDirectory(MoveLocation);
                        }
                        files.SaveAs(MoveLocation + Path.GetFileName((files.FileName)));
                        update.Image = Convert.ToString(MoveLocation + Path.GetFileName(files.FileName));
                    }
                }

                save = dbcontext.SaveChanges();

                if (tableData != null)
                {
                    var assignedAreas = JsonConvert.DeserializeObject<List<dynamic>>(tableData);

                    var assignedAreaStrings = assignedAreas.Select(item => (string)item.area).ToList();

                    var result = from area in dbcontext.Mas_AssignArea
                                 where area.Mas_LeaderFid == Leader.FID && !assignedAreaStrings.Contains(area.Area) && area.Deleted == false
                                 select area;
                    foreach (var area in result)
                    {
                        area.Deleted = true;  // Assuming 'Deleted' is the name of the column you want to update
                    }
                    dbcontext.SaveChanges();
                }

                if (Area != null)
                {
                    var GetFid = (from data in dbcontext.MAS_LEADER where data.FID == Leader.FID && data.MAS_CHC_FID == Leader.MAS_CHC_FID select data).FirstOrDefault();
                    int count = Area.Count;
                    for (int i = 0; i < count; i++)
                    {
                        Mas_AssignArea ASSArea = new Mas_AssignArea();
                        ASSArea.FDate = DateTime.Now;
                        ASSArea.MacID = LoginMachinId;
                        ASSArea.MacIP = LoginmachinIp;
                        ASSArea.Mas_LeaderFid = Leader.FID;
                        if (GetFid != null)
                        {
                            ASSArea.Mas_churchFId = Leader.MAS_CHC_FID;
                        }
                        ASSArea.StateFid = GetFid.StateFid;
                        ASSArea.City = GetFid.CityName;
                        //ASSArea.Area = string.Join(",", Area);
                        ASSArea.Area = Area[i];
                        ASSArea.Deleted = false;
                        dbcontext.Mas_AssignArea.Add(ASSArea);
                    }

                    save = dbcontext.SaveChanges();
                }

                if (save >= 0)
                {
                    TempData["Message"] = " Updated Successfully";
                    TempData["Icon"] = "success";
                    return RedirectToAction("LeaderCreateList", "LeaderCreateList", new { area = "Admin" });
                }
                else
                {
                    TempData["Message"] = "Not Update";
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
        #endregion

        #region DeleteLeader
        public ActionResult DeleteLeader(int? FID, int? MAS_CHC_FID)
        {
            try
            {
                if (Session["Admin"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
                int update = 0;
                var GetLeader = (from data in dbcontext.MAS_LEADER where data.FID == FID && data.MAS_CHC_FID == MAS_CHC_FID select data).FirstOrDefault();
                if (GetLeader != null)
                {
                    GetLeader.Status = false;
                    update = dbcontext.SaveChanges();
                }
                var GetLeaderAssignArea = (from data in dbcontext.Mas_AssignArea where data.Mas_LeaderFid == FID && data.Mas_churchFId == MAS_CHC_FID && data.Deleted == false select data).ToList();
                foreach (var item in GetLeaderAssignArea)
                {
                    item.Deleted = true;
                    update = dbcontext.SaveChanges();
                }
                if (update >= 0)
                {
                    TempData["Message"] = " Deleted Successfully";
                    TempData["Icon"] = "success";
                    return RedirectToAction("LeaderCreateList", "LeaderCreateList", new { area = "Admin" });
                }
                else
                {
                    TempData["Message"] = "Not Deleted";
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
        #endregion


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

                //Get Church Name Start

                //var GetPostCode = dbcontext.MAS_POSTCOD
                //  .Where(X => X.City == MasCityName && X.State_Fid == StateId )
                //  .Select(X => X.PostalCode)
                //  .FirstOrDefault();

                var MasPostcodeFid = dbcontext.MAS_POSTCOD
                        .Where(X => X.City == MasCityName && X.State_Fid == StateId)
                        .Select(X => X.Fid)
                        .ToList();

                var GetChurch = (from data in dbcontext.MAS_CHC
                                 where MasPostcodeFid.Contains((int)data.Mas_Postcode_Fid) && data.Mas_State_Fid == StateId
                                 select new BindDrop { Id = data.FID, Name = data.CHC_Name }).ToList();

                //Get Church Name end


                return Json(new { Data, GetChurch }, JsonRequestBehavior.AllowGet);
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