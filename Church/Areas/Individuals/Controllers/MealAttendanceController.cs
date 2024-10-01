using Church.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Church.Areas.Individuals.Controllers
{
    public class MealAttendanceController : Controller
    {
        CHC_NewEntities dbcontext = new CHC_NewEntities();
        DateTime TodaysDate = DateTime.Now;
        string LoginMachinId = Dns.GetHostName();
        // GET: Individuals/MealAttendance
        public ActionResult MealAttendance()
        {
            try
            {
                if (Session["U_Fid"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
                var IndvslFID = Session["U_Fid"];
                var IndvslName = Session["IndividualName"];
                var IndvslChurchFid = Session["IndvslUserCurchId"];
                int IndvslFid = Convert.ToInt32(IndvslFID);

                //var data = (from data in dbcontext)
                //var GetMember = (from data in dbcontext.MAS_INDVSL
                //                 join IndvlsFMLY in dbcontext.MAS_INDVFMLY on data.FID equals IndvlsFMLY.MAS_INDVSL_Fid
                //                 where data.Deactivate == false && IndvlsFMLY.Active == true && IndvlsFMLY.Member_id == IndvslFid
                //                 select new { data.IND_Name, data.FID };
                var GetMember = from data in dbcontext.MAS_INDVSL
                                join IndvlsFMLY in dbcontext.MAS_INDVFMLY
                                on data.FID equals IndvlsFMLY.Member_id
                                where data.Deactivate == false
                                      && IndvlsFMLY.Active == true
                                      && IndvlsFMLY.MAS_INDVSL_Fid == IndvslFid
                                select new
                                {
                                    data.IND_Name,
                                    data.FID
                                };

                var GetMemberName = GetMember.ToList();

                ViewBag.GetMemberNames = GetMemberName;


                //var GetMember = from data in dbcontext.MAS_INDVSL
                //                join IndvlsFMLY in dbcontext.MAS_INDVFMLY
                //                on data.FID equals IndvlsFMLY.Member_id
                //                where data.Deactivate == false
                //                      && IndvlsFMLY.Active == true
                //                      && IndvlsFMLY.MAS_INDVSL_Fid == IndvslFid
                //                select new
                //                {
                //                    data.IND_Name,
                //                    data.FID
                //                };

                //ViewBag.MealAttendanceQ = "Do you want meal after prayer?";

                return View();
            }
            catch (Exception ex)
            {

                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }          
        }

        [HttpPost]
        public ActionResult SaveMealAttendance(MealAttendance Atten, List<int> FamilyMemberFid, string Remark)
        {
            try
            {
                if (Session["U_Fid"] == null)
                {
                    return RedirectToAction("Login", "Home", new { area = "" });
                }

                int Save = 0;
                var LoginMachinIp = Dns.GetHostByName(LoginMachinId).AddressList[0].ToString();

                var IndvslFID = Session["U_Fid"];
                var IndvslName = Session["IndividualName"];
                var IndvslChurchFid = Session["IndvslUserCurchId"];

                MealAttendance mealAttendance = new MealAttendance();
                mealAttendance.MacID = LoginMachinId;
                mealAttendance.MacIP = LoginMachinIp;
                mealAttendance.FDate = DateTime.Now;
                mealAttendance.MemberFid = Convert.ToInt32(IndvslFID);
                mealAttendance.Mas_ChurchFid = Convert.ToInt32(IndvslChurchFid);
                //mealAttendance.FamilyMemberFid = string.Join(",", FamilyMemberFid);
                //mealAttendance.AttendanceRespone = MealAnswer;
                mealAttendance.Remark = Remark;
                dbcontext.MealAttendances.Add(mealAttendance);

                Save = dbcontext.SaveChanges();
                if(Save!=0)
                {
                    TempData["Message"] = "Attendance save";
                    TempData["Icon"] = "success";
                }
                else
                {
                    TempData["Message"] = "Attendance Not save";
                    TempData["Icon"] = "error";
                }
                return RedirectToAction("MealAttendance", "MealAttendance", new { area = "Individuals" });
         
            }
            catch (Exception ex)
            {

                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }
        }
    }
}