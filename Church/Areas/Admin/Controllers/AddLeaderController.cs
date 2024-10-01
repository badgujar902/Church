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
    public class AddLeaderController : Controller
    {
        CHC_NewEntities dbcontext = new CHC_NewEntities();
        DateTime TodaysDate = DateTime.Now;
        string LoginMachinId = Dns.GetHostName();
        // GET: Admin/AddLeader
        public ActionResult AddLeader()
        {
            try
            {
                if (Session["Admin"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
                var GetState = (from data in dbcontext.MAS_STATE where data.Active == 1 select new BindDrop { Id = data.Fid, Name = data.State }).ToList();
                ViewBag.GEtStateName = GetState;
                return View();
            }
            catch (Exception ex)
            {

                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }

        }
        [HttpPost]
        public ActionResult IsExistsData(string Email, string MobileNumber, string Area,string Position)
        {
            try
            {
              
                int error = 0;
                string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
                if (Email != "")
                {
                    if (!Regex.IsMatch(Email, pattern))
                    {
                        error = 1;
                        TempData["Message"] = "Enter Valid Email Address !";
                        TempData["Icon"] = "error";
                        TempData["Area"] = "";
                        return Json(new { Message = TempData["Message"], Icon = TempData["Icon"], Area = TempData["Area"] }, JsonRequestBehavior.AllowGet);
                    }
                    var GetEmail = dbcontext.MAS_LEADER.Where(X => X.Lead_EmailID == Email && X.Lead_Mob == MobileNumber).FirstOrDefault();
                    if (GetEmail != null)
                    {
                        if (GetEmail.Lead_EmailID != null)
                        {
                            error = 1;
                            TempData["Message"] = "Email Already Exist";
                            TempData["Icon"] = "error";
                            TempData["Area"] = "";
                            return Json(new { Message = TempData["Message"], Icon = TempData["Icon"], Area= TempData["Area"] }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    if(Position!= "Head Pastor")
                    {
                        var GetArea = (from data in dbcontext.Mas_AssignArea where data.Area == Area && data.Deleted == false select new { data.Area }).FirstOrDefault();
                        if (GetArea!=null)
                        {
                            TempData["Message"] = "Area Already Assigned to Leader";
                            TempData["Icon"] = "error";
                            TempData["Area"] = GetArea.Area;/*string.Join(",", GetArea.Area);*/
                            return Json(new { Message = TempData["Message"], Icon = TempData["Icon"], Area = TempData["Area"] });
                        }
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
        public ActionResult SaveAddLeader(MAS_LEADER Leader, string UserName, string Password, string Postion, HttpPostedFileBase file)
        {
            try
            {
                if (Session["Admin"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
                int save = 0;
                HttpPostedFile files = System.Web.HttpContext.Current.Request.Files["file"];
                var LoginmachinIp = Dns.GetHostByName(LoginMachinId).AddressList[0].ToString();
                if (Leader != null)
                {
                    MAS_LEADER leader = new MAS_LEADER();
                    leader.FDate = DateTime.Now;
                    leader.MacID = LoginMachinId;
                    leader.MacIP = LoginmachinIp;
                    leader.StateFid = Leader.StateFid;
                    leader.CityName = Leader.CityName;
                    //leader.ChurchArea = Leader.ChurchArea;
                    leader.PostCode = Leader.PostCode;
                    leader.MAS_CHC_FID = Leader.MAS_CHC_FID;
                    leader.Lead_Name = Leader.Lead_Name;
                    leader.Lead_Mob = Leader.Lead_Mob;
                    leader.Lead_EmailID = Leader.Lead_EmailID;
                    leader.Lead_Add = Leader.Lead_Add;
                    leader.Lead_DOJ = DateTime.Now;
                    leader.Lead_DOB = Leader.Lead_DOB;
                    leader.Gender = Leader.Gender;
                    leader.Status = true;

                    var filelctn = Leader.Lead_Mob;
                    string MoveLocation = @"C:\StealthChurch\LeaderCreatetionData\\" + filelctn + "\\";

                    if (files.ContentLength > 0)
                    {
                        if (!Directory.Exists(MoveLocation))
                        {
                            Directory.CreateDirectory(MoveLocation);
                        }
                        files.SaveAs(MoveLocation + Path.GetFileName((files.FileName)));
                        leader.Image = Convert.ToString(MoveLocation + Path.GetFileName(files.FileName));
                    }

                    dbcontext.MAS_LEADER.Add(leader);
                    save = dbcontext.SaveChanges();


                    if (Postion != "Head Pastor")
                    {
                        if (save != 0)
                        {
                            var leaderFid = (from data in dbcontext.MAS_LEADER where data.MAS_CHC_FID == Leader.MAS_CHC_FID && data.Lead_Mob == Leader.Lead_Mob select data).FirstOrDefault();
                            //MAS_UID userid = new MAS_UID();
                            Mas_AssignArea AssignArea = new Mas_AssignArea();
                            AssignArea.FDate = DateTime.Now;
                            AssignArea.MacID = LoginMachinId;
                            AssignArea.MacIP = LoginmachinIp;
                            AssignArea.Mas_LeaderFid = Convert.ToInt32(leaderFid.FID);
                            AssignArea.Mas_churchFId = Leader.MAS_CHC_FID;
                            AssignArea.StateFid = leaderFid.StateFid;
                            AssignArea.City = leader.CityName;
                            AssignArea.Area = Leader.ChurchArea;
                            AssignArea.Deleted = false;
                            dbcontext.Mas_AssignArea.Add(AssignArea);
                            save = dbcontext.SaveChanges();
                        }


                    }

                    if (save != 0)
                    {
                        var leaderFid = (from data in dbcontext.MAS_LEADER where data.MAS_CHC_FID == Leader.MAS_CHC_FID && data.Lead_Mob == Leader.Lead_Mob select data).FirstOrDefault();
                        MAS_UID userid = new MAS_UID();
                        userid.FDate = DateTime.Now;
                        userid.MacID = LoginMachinId;
                        userid.MacIP = LoginmachinIp;
                        userid.MAS_LEADER_FID = Convert.ToInt32(leaderFid.FID);
                        userid.MAS_CHC_FID = Leader.MAS_CHC_FID;
                        userid.U_ID = UserName;
                        userid.U_Pass = Password;
                        //userid.U_Type = "Leader";
                        if(Postion== "Head Pastor")
                        {
                            userid.U_Type = "Head Leader";
                        }
                        else
                        {
                            userid.U_Type = "Leader";
                        }
                        
                        userid.U_Status = true;
                        dbcontext.MAS_UID.Add(userid);
                        save = dbcontext.SaveChanges();

                    }

                }
                if (save != 0)
                {
                    TempData["Message"] = "Leader Created Successfully";
                    TempData["Icon"] = "success";
                    return RedirectToAction("AddLeader", "AddLeader", new { area = "Admin" });
                }
                else
                {
                    TempData["Message"] = "Leader not Created";
                    TempData["Icon"] = "error";
                    return RedirectToAction("AddLeader", "AddLeader", new { area = "Admin" });
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

        //#region GetChurchName
        //public ActionResult GetChurchName(int? StateId, string MasCityName,string CityArea/*,string PostCode*/ )
        //{
        //    try
        //    {
        //        if (Session["Admin"] == null)
        //        {
        //            return RedirectToAction("Login", "Home", new { area = "" });
        //        }

        //        var GetPostCode = dbcontext.MAS_POSTCOD
        //            .Where(X => X.City == MasCityName && X.State_Fid == StateId && X.Area == CityArea)
        //            .Select(X => X.PostalCode)
        //            .FirstOrDefault();

        //        var MasPostcodeFid = dbcontext.MAS_POSTCOD
        //                .Where(X => X.City == MasCityName && X.State_Fid == StateId && X.Area == CityArea && X.PostalCode == GetPostCode)
        //                .Select(X => X.Fid)
        //                .ToList();
        //        //var MasPostcodeFid = dbcontext.MAS_POSTCOD
        //        //       .Where(X => X.City == MasCityName && X.State_Fid == StateId && X.Area == CityArea)
        //        //       .Select(X => X.Fid)
        //        //       .ToList();

        //        //var GetPostCode=(from data in dbcontext.MAS_POSTCOD)



        //        //MasPostcodeFid
        //        var Data = (from data in dbcontext.MAS_CHC
        //                    where MasPostcodeFid.Contains((int)data.Mas_Postcode_Fid) && data.Mas_State_Fid == StateId
        //                    select new BindDrop { Id = data.FID, Name = data.CHC_Name }).ToList();


        //        return Json(Data, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        @Session["GetErrorMessage"] = ex.Message;
        //        return RedirectToAction("Error", "Home", new { area = "" });
        //    }
        //}
        //#endregion

        //#region GetArea
        //public ActionResult GetArea(int? StateId, string MasCityName)
        //{
        //    try
        //    {
        //        if (Session["Admin"] == null)
        //        {
        //            return RedirectToAction("Login", "Home", new { area = "" });
        //        }
        //        var Data = (from data in dbcontext.MAS_POSTCOD
        //                    where data.State_Fid == StateId && data.City == MasCityName
        //                    select new BindDrop { Name = data.Area })
        //                     .Distinct()
        //                    .ToList();

        //        //var Data = dbcontext.MAS_POSTCOD
        //        //  .Where(data => data.State_Fid == StateId && data.City == MasCityName)
        //        //  .GroupBy(data => data.Area)
        //        //  .Select(group => new BindDrop { Id = group.First().Fid, Name = group.Key })
        //        //  .ToList();


        //        return Json(Data, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        @Session["GetErrorMessage"] = ex.Message;
        //        return RedirectToAction("Error", "Home", new { area = "" });
        //    }
        //}
        //#endregion
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

        //#region PostCode
        //public ActionResult PostCode(int? StateId, string CityArea,string MasCityName)
        //{
        //    try
        //    {
        //        if (Session["Admin"] == null)
        //        {
        //            return RedirectToAction("Login", "Home", new { area = "" });
        //        }
        //        //var Data = (from data in dbcontext.MAS_POSTCOD
        //        //            where data.State_Fid == StateId && data.Area == CityArea && data.City== MasCityName
        //        //            select new BindDrop { Name = data.PostalCode })
        //        //             .Distinct()
        //        //            .ToList();

        //        var Data = (from data in dbcontext.MAS_POSTCOD
        //                    where data.State_Fid == StateId && data.Area == CityArea && data.City == MasCityName
        //                    group data by data.Area into groupedData
        //                    select new BindDrop
        //                    {
        //                        Name = groupedData.FirstOrDefault().PostalCode
        //                    })
        //    .ToList();

        //        return Json(Data, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        @Session["GetErrorMessage"] = ex.Message;
        //        return RedirectToAction("Error", "Home", new { area = "" });
        //    }
        //}
        //#endregion
    }

}