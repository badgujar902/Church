using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Church.Models;
using System.Net;
using System.IO;
using Church.Areas.Admin.Models;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Data.Entity;
using System.Net.Mail;

namespace Church.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        CHC_NewEntities dbcontext = new CHC_NewEntities();
        DateTime TodaysDate = DateTime.Now;
        string LoginMachinId = Dns.GetHostName();
        #region Login
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string username, string password, MAS_UID UID)
        {

            DateTime FormDate = DateTime.Now;
            Session["UserName"] = "Admin";
            Session["AppCallCalled"] = "WebCall";

            try
            {
                var HeadLeaderId = (from data in dbcontext.MAS_UID where data.U_ID == username && data.U_Pass == password && data.U_Type == "Head Leader" && data.U_Status == true select data).FirstOrDefault();
                var LeaderId = (from data in dbcontext.MAS_UID where data.U_ID == username && data.U_Pass == password && data.U_Type == "Leader" && data.U_Status == true select data).FirstOrDefault();
                var IndividualId = (from data in dbcontext.MAS_UID where data.U_ID == username && data.U_Pass == password && data.U_Type == "Individual" && data.U_Status == true select data).FirstOrDefault();
                if (username == "admin" && password == "admin")
                {
                    Session["Admin"] = "admin";

                    return RedirectToAction("Dashboard", "Admin", new { area = "Admin" });
                }
                else if (HeadLeaderId != null)
                {
                    var LeadId = HeadLeaderId.MAS_LEADER_FID;
                    int LeaderFid = Convert.ToInt32(LeadId);
                    var LeadName = dbcontext.MAS_LEADER.Where(X => X.FID == LeadId).Select(X => X.Lead_Name).FirstOrDefault();
                    Session["LeaderName"] = LeadName;
                    Session["HeadLeader_U_Fid"] = HeadLeaderId.MAS_LEADER_FID;
                    var LeaderChurchFId = HeadLeaderId.MAS_CHC_FID;

                    var LeaderchurchName = dbcontext.MAS_CHC.Where(x => x.FID == LeaderChurchFId && x.Status == true).Select(x => x.CHC_Name).FirstOrDefault();
                    Session["HeadLeaderCurchName"] = LeaderchurchName;
                    Session["HeadLeaderCurchId"] = LeaderChurchFId;
                    Session["HeadLeaderFId"] = LeadId;
                    Session["LeaderCurchName"] = LeaderchurchName;
                    //Session["HeadLeaderDesignation"] = LeaderId.U_Type;

                    Session["LeaderDesignation"] = HeadLeaderId.U_Type;


                    var GetLeaderProfileData = (from data in dbcontext.MAS_LEADER where data.FID == LeaderFid && data.Status == true select data).FirstOrDefault();

                    if (GetLeaderProfileData != null)
                    {
                        var BasUrl = (from data in dbcontext.DocumentBaseUrls select data).FirstOrDefault();
                        if (BasUrl != null)
                        {
                            string ImageUrl = GetLeaderProfileData.Image;
                            if (ImageUrl != null)
                            {
                                string fileName = Path.GetFileName(ImageUrl);
                                string baseUrl = BasUrl.BaseUrl;
                                string relativePath = ImageUrl.Replace("C:\\", "").Replace("\\", "/");
                                string finalImageUrl = baseUrl + relativePath;
                                ViewBag.LeaderProfileUrl = finalImageUrl;
                                Session["LeaderProfile"] = finalImageUrl;
                            }
                            else
                            {
                                Session["LeaderProfile"] = null;
                            }
                        }
                        else
                        {
                            Session["LeaderProfile"] = "";
                        }

                    }


                    return RedirectToAction("Dashboard", "Leader", new { area = "Leader" });
                }
                else if (LeaderId != null)
                {
                    var LeadId = LeaderId.MAS_LEADER_FID;
                    int LeaderFid = Convert.ToInt32(LeadId);
                    var LeadName = dbcontext.MAS_LEADER.Where(X => X.FID == LeadId).Select(X => X.Lead_Name).FirstOrDefault();
                    Session["LeaderName"] = LeadName;
                    Session["U_Fid"] = LeaderId.FID;
                    var LeaderChurchFId = LeaderId.MAS_CHC_FID;

                    var LeaderchurchName = dbcontext.MAS_CHC.Where(x => x.FID == LeaderChurchFId && x.Status == true).Select(x => x.CHC_Name).FirstOrDefault();
                    Session["LeaderCurchName"] = LeaderchurchName;
                    Session["LeaderCurchId"] = LeaderChurchFId;
                    Session["LeaderFId"] = LeadId;
                    Session["LeaderDesignation"] = LeaderId.U_Type;

                    var GetLeaderProfileData = (from data in dbcontext.MAS_LEADER where data.FID == LeaderFid && data.Status == true select data).FirstOrDefault();

                    if (GetLeaderProfileData != null)
                    {
                        var BasUrl = (from data in dbcontext.DocumentBaseUrls select data).FirstOrDefault();
                        if (BasUrl != null)
                        {
                            string ImageUrl = GetLeaderProfileData.Image;
                            if (ImageUrl != null)
                            {
                                string fileName = Path.GetFileName(ImageUrl);
                                string baseUrl = BasUrl.BaseUrl;
                                string relativePath = ImageUrl.Replace("C:\\", "").Replace("\\", "/");
                                string finalImageUrl = baseUrl + relativePath;
                                ViewBag.LeaderProfileUrl = finalImageUrl;
                                Session["LeaderProfile"] = finalImageUrl;
                            }
                            else
                            {                               
                                Session["LeaderProfile"] = null;
                            }
                        }
                        else
                        {                            
                            Session["LeaderProfile"] = "";
                        }

                    }

                    return RedirectToAction("Dashboard", "Leader", new { area = "Leader" });
                }
                else if (IndividualId != null)
                {
                    var IndvId = IndividualId.MAS_INDVSL_FID;
                    var IndvName = dbcontext.MAS_INDVSL.Where(X => X.FID == IndvId).Select(X => X.IND_Name).FirstOrDefault();
                    Session["IndividualName"] = IndvName;
                    Session["U_Fid"] = IndividualId.MAS_INDVSL_FID;
                    var INDVSlCurchID = IndividualId.MAS_CHC_FID;
                    Session["IndvslUserCurchId"] = IndividualId.MAS_CHC_FID;
                    var INDVSLchurchName = dbcontext.MAS_CHC.Where(x => x.FID == INDVSlCurchID && x.Status == true).Select(x => x.CHC_Name).FirstOrDefault();
                    Session["IndVslCurchName"] = INDVSLchurchName;
                    if (IndividualId.U_Type == "Individual")
                    {
                        Session["MemberDesignation"] = "Member";
                    }

                    var indvslId = Session["U_Fid"];
                    int IndvslFid = Convert.ToInt32(indvslId);

                    var GetMemberProfileData = (from data in dbcontext.MAS_INDVSL where data.FID == IndvslFid && data.Deactivate == false && data.Status == false select data).FirstOrDefault();

                    if (GetMemberProfileData != null)
                    {
                        var BasUrl = (from data in dbcontext.DocumentBaseUrls select data).FirstOrDefault();
                        if (BasUrl != null)
                        {
                            string ImageUrl = GetMemberProfileData.IND_Image;
                            if (ImageUrl != null)
                            {
                                string fileName = Path.GetFileName(ImageUrl);
                                string baseUrl = BasUrl.BaseUrl;
                                string relativePath = ImageUrl.Replace("C:\\", "").Replace("\\", "/");
                                string finalImageUrl = baseUrl + relativePath;                               
                                Session["MemberProfileUrl"] = finalImageUrl;
                            }
                            else
                            {                               
                                Session["MemberProfileUrl"] = null;
                            }
                        }
                        else
                        {
                            Session["MemberProfileUrl"] = "";
                        }
                    }
                    return RedirectToAction("Dashboard", "Individuals", new { area = "Individuals" });
                }
                else
                {
                    TempData["Error"] = "Invalid User Name Or Password !!!";
                    return View();
                }
            }
            catch (Exception Ex)
            {
                TempData["Error"] = "Oops,Something went wrong!!!";

                return View();
            }

        }
        #endregion


        #region MobileCall
        public ActionResult AppCall(string username, string password)
        {

            DateTime FormDate = DateTime.Now;
            Session["UserName"] = "Admin";

            Session["AppCallCalled"] = "AppCall";

            try
            {
                var HeadLeaderId = (from data in dbcontext.MAS_UID where data.U_ID == username && data.U_Pass == password && data.U_Type == "Head Leader" && data.U_Status == true select data).FirstOrDefault();
                var LeaderId = (from data in dbcontext.MAS_UID where data.U_ID == username && data.U_Pass == password && data.U_Type == "Leader" && data.U_Status == true select data).FirstOrDefault();
                var IndividualId = (from data in dbcontext.MAS_UID where data.U_ID == username && data.U_Pass == password && data.U_Type == "Individual" && data.U_Status == true select data).FirstOrDefault();
                if (username == "admin" && password == "admin")
                {
                    Session["Admin"] = "admin";

                    return RedirectToAction("Dashboard", "Admin", new { area = "Admin" });
                }
                else if (HeadLeaderId != null)
                {
                    var LeadId = HeadLeaderId.MAS_LEADER_FID;
                    int LeaderFid = Convert.ToInt32(LeadId);
                    var LeadName = dbcontext.MAS_LEADER.Where(X => X.FID == LeadId).Select(X => X.Lead_Name).FirstOrDefault();
                    Session["LeaderName"] = LeadName;
                    Session["HeadLeader_U_Fid"] = HeadLeaderId.MAS_LEADER_FID;
                    var LeaderChurchFId = HeadLeaderId.MAS_CHC_FID;

                    var LeaderchurchName = dbcontext.MAS_CHC.Where(x => x.FID == LeaderChurchFId && x.Status == true).Select(x => x.CHC_Name).FirstOrDefault();
                    Session["HeadLeaderCurchName"] = LeaderchurchName;
                    Session["HeadLeaderCurchId"] = LeaderChurchFId;
                    Session["HeadLeaderFId"] = LeadId;
                    Session["LeaderCurchName"] = LeaderchurchName;
                    //Session["HeadLeaderDesignation"] = LeaderId.U_Type;

                    Session["LeaderDesignation"] = HeadLeaderId.U_Type;


                    var GetLeaderProfileData = (from data in dbcontext.MAS_LEADER where data.FID == LeaderFid && data.Status == true select data).FirstOrDefault();

                    if (GetLeaderProfileData != null)
                    {
                        var BasUrl = (from data in dbcontext.DocumentBaseUrls select data).FirstOrDefault();
                        if (BasUrl != null)
                        {
                            string ImageUrl = GetLeaderProfileData.Image;
                            if (ImageUrl != null)
                            {
                                string fileName = Path.GetFileName(ImageUrl);
                                string baseUrl = BasUrl.BaseUrl;
                                string relativePath = ImageUrl.Replace("C:\\", "").Replace("\\", "/");
                                string finalImageUrl = baseUrl + relativePath;
                                ViewBag.LeaderProfileUrl = finalImageUrl;
                                Session["LeaderProfile"] = finalImageUrl;
                            }
                            else
                            {
                                Session["LeaderProfile"] = null;
                            }
                        }
                        else
                        {
                            Session["LeaderProfile"] = "";
                        }

                    }


                    return RedirectToAction("Dashboard", "Leader", new { area = "Leader" });
                }
                else if (LeaderId != null)
                {
                    var LeadId = LeaderId.MAS_LEADER_FID;
                    int LeaderFid = Convert.ToInt32(LeadId);
                    var LeadName = dbcontext.MAS_LEADER.Where(X => X.FID == LeadId).Select(X => X.Lead_Name).FirstOrDefault();
                    Session["LeaderName"] = LeadName;
                    Session["U_Fid"] = LeaderId.FID;
                    var LeaderChurchFId = LeaderId.MAS_CHC_FID;

                    var LeaderchurchName = dbcontext.MAS_CHC.Where(x => x.FID == LeaderChurchFId && x.Status == true).Select(x => x.CHC_Name).FirstOrDefault();
                    Session["LeaderCurchName"] = LeaderchurchName;
                    Session["LeaderCurchId"] = LeaderChurchFId;
                    Session["LeaderFId"] = LeadId;
                    Session["LeaderDesignation"] = LeaderId.U_Type;

                    var GetLeaderProfileData = (from data in dbcontext.MAS_LEADER where data.FID == LeaderFid && data.Status == true select data).FirstOrDefault();

                    if (GetLeaderProfileData != null)
                    {
                        var BasUrl = (from data in dbcontext.DocumentBaseUrls select data).FirstOrDefault();
                        if (BasUrl != null)
                        {
                            string ImageUrl = GetLeaderProfileData.Image;
                            if (ImageUrl != null)
                            {
                                string fileName = Path.GetFileName(ImageUrl);
                                string baseUrl = BasUrl.BaseUrl;
                                string relativePath = ImageUrl.Replace("C:\\", "").Replace("\\", "/");
                                string finalImageUrl = baseUrl + relativePath;
                                ViewBag.LeaderProfileUrl = finalImageUrl;
                                Session["LeaderProfile"] = finalImageUrl;
                            }
                            else
                            {
                                Session["LeaderProfile"] = null;
                            }
                        }
                        else
                        {
                            Session["LeaderProfile"] = "";
                        }

                    }

                    return RedirectToAction("Dashboard", "Leader", new { area = "Leader" });
                }
                else if (IndividualId != null)
                {
                    var IndvId = IndividualId.MAS_INDVSL_FID;
                    var IndvName = dbcontext.MAS_INDVSL.Where(X => X.FID == IndvId).Select(X => X.IND_Name).FirstOrDefault();
                    Session["IndividualName"] = IndvName;
                    Session["U_Fid"] = IndividualId.MAS_INDVSL_FID;
                    var INDVSlCurchID = IndividualId.MAS_CHC_FID;
                    Session["IndvslUserCurchId"] = IndividualId.MAS_CHC_FID;
                    var INDVSLchurchName = dbcontext.MAS_CHC.Where(x => x.FID == INDVSlCurchID && x.Status == true).Select(x => x.CHC_Name).FirstOrDefault();
                    Session["IndVslCurchName"] = INDVSLchurchName;
                    if (IndividualId.U_Type == "Individual")
                    {
                        Session["MemberDesignation"] = "Member";
                    }

                    var indvslId = Session["U_Fid"];
                    int IndvslFid = Convert.ToInt32(indvslId);

                    var GetMemberProfileData = (from data in dbcontext.MAS_INDVSL where data.FID == IndvslFid && data.Deactivate == false && data.Status == false select data).FirstOrDefault();

                    if (GetMemberProfileData != null)
                    {
                        var BasUrl = (from data in dbcontext.DocumentBaseUrls select data).FirstOrDefault();
                        if (BasUrl != null)
                        {
                            string ImageUrl = GetMemberProfileData.IND_Image;
                            if (ImageUrl != null)
                            {
                                string fileName = Path.GetFileName(ImageUrl);
                                string baseUrl = BasUrl.BaseUrl;
                                string relativePath = ImageUrl.Replace("C:\\", "").Replace("\\", "/");
                                string finalImageUrl = baseUrl + relativePath;
                                Session["MemberProfileUrl"] = finalImageUrl;
                            }
                            else
                            {
                                Session["MemberProfileUrl"] = null;
                            }
                        }
                        else
                        {
                            Session["MemberProfileUrl"] = "";
                        }
                    }
                    return RedirectToAction("Dashboard", "Individuals", new { area = "Individuals" });
                }
                else
                {
                    TempData["Error"] = "Invalid User Name Or Password !!!";
                    return View();
                }
            }
            catch (Exception Ex)
            {
                TempData["Error"] = "Oops,Something went wrong!!!";

                return View();
            }

        }
        #endregion

        #region Registration
        public ActionResult RegistrationForm()
        {
            try
            {
                //var church = dbcontext.MAS_CHC.Where(X => X.Status == true).ToList();
                //ViewBag.ChurchList = church;

                var GetState = (from data in dbcontext.MAS_STATE where data.Active == 1 select new BindDrop { Id = data.Fid, Name = data.State }).ToList();
                ViewBag.GEtStateName = GetState;
                if (ViewBag.GEtStateName == null)
                {
                    ViewBag.GEtStateName = null;
                }

                TempData["doj"] = TodaysDate;
                return View();
            }
            catch (Exception ex)
            {
                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }
        }
        #region IsExists
        public ActionResult IsExists(string ContactNo, string EmailId)
        {
            try
            {

                string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
                if (EmailId != "")
                {
                    if (!Regex.IsMatch(EmailId, pattern))
                    {
                        TempData["Message"] = "Enter Valid Email Address !";
                        TempData["Icon"] = "error";
                        return Json(new { Message = TempData["Message"], Icon = TempData["Icon"] }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                int error = 0;
                if (EmailId != "")
                {
                    //var Data = dbcontext.MAS_INDVSL.Where(X => X.AdharNo == AdharNo).FirstOrDefault();
                    var GetEmail = dbcontext.MAS_INDVSL.Where(X => X.IND_Email == EmailId).FirstOrDefault();
                    var GetMobileNumber = dbcontext.MAS_INDVSL.Where(X => X.IND_Mob == ContactNo).FirstOrDefault();

                    //if (Data != null)
                    //{
                    //    if (Data.AdharNo != null)
                    //    {
                    //        error = 1;
                    //        TempData["Message"] = "Enter Valid Adhar No  !";
                    //        TempData["Icon"] = "error";
                    //        return Json(new { Message = TempData["Message"], Icon = TempData["Icon"] }, JsonRequestBehavior.AllowGet);
                    //    }
                    //}
                    if (GetEmail != null)
                    {
                        if (GetEmail.IND_Email != null)
                        {
                            error = 1;
                            TempData["Message"] = "Email Already Exist";
                            TempData["Icon"] = "error";
                            return Json(new { Message = TempData["Message"], Icon = TempData["Icon"] }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    if (GetMobileNumber != null)
                    {
                        if (GetMobileNumber.IND_Mob != null)
                        {
                            error = 1;
                            TempData["Message"] = "Mobile Number Can not be Duplicate !";
                            TempData["Icon"] = "error";
                            return Json(new { Message = TempData["Message"], Icon = TempData["Icon"] }, JsonRequestBehavior.AllowGet);
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
        #endregion
        //[HttpPost]
        //public ActionResult SaveRegistrationForm(MAS_INDVSL Ind, HttpPostedFileBase file)
        //{
        //    var LoginmachinIp = Dns.GetHostByName(LoginMachinId).AddressList[0].ToString();
        //    try
        //    {
        //        HttpPostedFile files = System.Web.HttpContext.Current.Request.Files["file"];
        //        int i = 0;
        //        if (files != null && files.ContentLength > 0)
        //        {
        //            List<string> allowedExtensions = new List<string> { ".jpg", ".jpeg", ".png" };
        //            // Get the file extension
        //            string fileExtension = Path.GetExtension(files.FileName);
        //            if (allowedExtensions.Contains(fileExtension.ToLower()))
        //            {
        //                MAS_INDVSL individual = new MAS_INDVSL();
        //                individual.FDate = TodaysDate;
        //                individual.MacID = LoginMachinId;
        //                individual.MacIP = LoginmachinIp;
        //                individual.MAS_CHC_FID = Ind.MAS_CHC_FID;
        //                individual.StateFid = Ind.StateFid;
        //                individual.City = Ind.City;
        //                individual.Area = Ind.Area;
        //                individual.IND_Name = Ind.IND_Name;
        //                individual.IND_Mob = Ind.IND_Mob;
        //                individual.IND_Email = Ind.IND_Email;
        //                individual.IND_Address = Ind.IND_Address;
        //                //individual.IND_DOJ = DateTime.Now;
        //                individual.IND_DOB = Ind.IND_DOB;
        //                //individual.AdharNo = Ind.AdharNo;
        //                //individual.NameAsAadhar = Ind.NameAsAadhar;
        //                individual.Gender = Ind.Gender;
        //                individual.Deactivate = false;
        //                individual.Enroll_Type = "Self";
        //                var DocId = ((from db in dbcontext.MAS_INDVSL select (int?)db.Enroll_ID).Max() ?? 0) + 1;
        //                individual.Enroll_ID = DocId;
        //                individual.Status = true;
        //                dbcontext.MAS_INDVSL.Add(individual);
        //                //dbcontext.SaveChanges();

        //                var filelctn = individual.IND_Mob;
        //                string MoveLocation = @"C:\StealthChurch\RegistrationData\\" + filelctn + "\\";

        //                if (files.ContentLength > 0)
        //                {
        //                    if (!Directory.Exists(MoveLocation))
        //                    {
        //                        Directory.CreateDirectory(MoveLocation);
        //                    }
        //                    files.SaveAs(MoveLocation + Path.GetFileName((files.FileName)));
        //                    individual.IND_Image = Convert.ToString(MoveLocation + Path.GetFileName(files.FileName));
        //                }
        //                i = dbcontext.SaveChanges();
        //            }
        //            else
        //            {
        //                TempData["Message"] = "Please select Only .jpg, .jpeg, .png";
        //                TempData["Icon"] = "error";
        //                return RedirectToAction("RegistrationForm", "Home");
        //            }

        //            // Use fileExtension as needed (e.g., save to disk, validate against allowed extensions, etc.)
        //            //Console.WriteLine("File extension: " + fileExtension);
        //        }

        //        if (i >= 1)
        //        {
        //            TempData["Message"] = "Registration Completed Successfully";
        //            TempData["Icon"] = "success";
        //            return RedirectToAction("Login", "Home");
        //        }
        //        else
        //        {
        //            TempData["Message"] = "Registration Not Completed Successfully";
        //            TempData["Icon"] = "error";
        //            return RedirectToAction("Login", "Home");

        //        }

        //    }
        //    catch (Exception Ex)
        //    {
        //        return View();
        //    }
        //}

        [HttpPost]
        public async Task<ActionResult> SaveRegistrationForm(MAS_INDVSL Ind, HttpPostedFileBase file)
        {
            var LoginmachinIp = Dns.GetHostByName(LoginMachinId).AddressList[0].ToString();

            int success = 0;
            try
            {
                if (file != null && file.ContentLength > 0)
                {
                    // Define the request URI
                    string requestUri = "http://49.248.250.54:8081/api/NewRegistration";

                    // Authorization token
                    string token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJleHAiOjE3NTE2OTkzNzMsImlzcyI6Imh0dHA6Ly9DaHVyY2guY29tIiwiYXVkIjoiaHR0cDovL0NodXJjaC5jb20ifQ.BGZr5r5swcj8KT6dhc15mt14IWqUsKWNPdgfoWLqyVc";

                    using (var client = new HttpClient())
                    {
                        // Configure timeout settings
                        client.Timeout = TimeSpan.FromMinutes(10); // Set a reasonable timeout value                  

                        var formData = new MultipartFormDataContent();
                        formData.Add(new StringContent(LoginMachinId), "macID");
                        formData.Add(new StringContent(LoginmachinIp), "macIP");
                        formData.Add(new StringContent(Ind.StateFid.ToString()), "StateFid");
                        formData.Add(new StringContent(Ind.City), "city");
                        formData.Add(new StringContent(Ind.Area), "area");
                        formData.Add(new StringContent(Ind.MAS_CHC_FID.ToString()), "mAS_CHC_FID");
                        formData.Add(new StringContent(Ind.IND_Name), "IND_Name");
                        formData.Add(new StringContent(Ind.IND_Mob), "IND_Mob");
                        formData.Add(new StringContent(Ind.IND_Email), "IND_Email");
                        formData.Add(new StringContent(Ind.IND_Address), "IND_Address");
                        formData.Add(new StringContent(Ind.IND_DOB.ToString()), "IND_DOB");
                        formData.Add(new StringContent(Ind.Gender), "Gender");


                        // Add the file content
                        var fileContent = new StreamContent(file.InputStream);
                        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
                        formData.Add(fileContent, "file", file.FileName);
                        // Create the request message
                        using (var request = new HttpRequestMessage(HttpMethod.Post, requestUri))
                        {
                            request.Content = formData;
                            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                            // Send the request
                            using (var response = await client.SendAsync(request))
                            {
                                if (response.IsSuccessStatusCode)
                                {
                                    string responseContent = await response.Content.ReadAsStringAsync();
                                    var obj = JsonConvert.DeserializeObject<dynamic>(responseContent);
                                    success = 1;
                                    TempData["Message"] = "Registered Successfully";
                                    TempData["Icon"] = "success";
                                    return RedirectToAction("Login", "Home");
                                }
                                else
                                {
                                    string responseContent2 = await response.Content.ReadAsStringAsync();
                                    TempData["Message"] = "Something went wrong";
                                    TempData["Icon"] = "error";
                                    return RedirectToAction("RegistrationForm", "Home");
                                }
                            }
                        }
                    }
                }
                return RedirectToAction("RegistrationForm", "Home");
            }
            catch (Exception ex)
            {
                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });

            }
        }

        #endregion Registration

        public ActionResult Error()
        {
            try
            {
                return View();
            }
            catch (Exception Ex)
            {
                return View();
            }
        }



        #region GetCity
        public ActionResult GetCity(int? StateId)
        {
            try
            {

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
        //public ActionResult GetChurchName(int? StateId, string MasCityName, string CityArea, string PostCode)
        //{
        //    try
        //    {


        //        //var MasPostcodeFid = dbcontext.MAS_POSTCOD
        //        //         .Where(X => X.City == MasCityName && X.State_Fid == StateId)
        //        //         .Select(X => X.Fid)
        //        //         .ToList();

        //        var MasPostcodeFid = dbcontext.MAS_POSTCOD
        //                .Where(X => X.City == MasCityName && X.State_Fid == StateId && X.Area == CityArea && X.PostalCode == PostCode)
        //                .Select(X => X.Fid)
        //                .ToList();


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


        #region GetArea
        public ActionResult GetArea(int? StateId, string MasCityName)
        {
            try
            {

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

        //#region GetArea
        //public ActionResult GetArea(int? StateId, string MasCityName)
        //{
        //    try
        //    {

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


        #region PostCode
        public ActionResult PostCode(int? StateId, string CityArea, string MasCityName)
        {
            try
            {
                //var Data = (from data in dbcontext.MAS_POSTCOD
                //            where data.State_Fid == StateId && data.Area == CityArea && data.City== MasCityName
                //            select new BindDrop { Name = data.PostalCode })
                //             .Distinct()
                //            .ToList();

                var Data = (from data in dbcontext.MAS_POSTCOD
                            where data.State_Fid == StateId && data.Area == CityArea && data.City == MasCityName
                            group data by data.Area into groupedData
                            select new BindDrop
                            {
                                Name = groupedData.FirstOrDefault().PostalCode
                            }).ToList();

                //var postcode = Data[0].Name;

                //var MasPostcodeFid = dbcontext.MAS_POSTCOD
                //     .Where(X => X.City == MasCityName && X.State_Fid == StateId && X.Area == CityArea && X.PostalCode == postcode)
                //     .Select(X => X.Fid)
                //     .ToList();


                ////MasPostcodeFid
                //var GetChurch = (from data in dbcontext.MAS_CHC
                //            where MasPostcodeFid.Contains((int)data.Mas_Postcode_Fid) && data.Mas_State_Fid == StateId
                //            select new BindDrop { Id = data.FID, Name = data.CHC_Name }).ToList();


                return Json(Data = Data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                @Session["GetErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Home", new { area = "" });
            }
        }
        #endregion

        #region ForgotPassword 
        public ActionResult CheckUserIsValid(string email, string UserId, string pass)
        {
            try
            {
                if (email != null && UserId != null)
                {
                    var ForgotData = (from data in dbcontext.MAS_INDVSL where data.IND_Email == email && data.Status == false select data).FirstOrDefault();
                    if (ForgotData != null)
                    {
                        return Json(true, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        TempData["Message"] = "Invalid Email Id";
                        TempData["Icon"] = "error";
                        return Json(new { Message = TempData["Message"], Icon = TempData["Icon"] }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    TempData["Message"] = "Enter Email";
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


        public ActionResult ForgotPassword(string email, string newUserId, string newPassword)
        {
            try
            {
                if (email != null)
                {
                    var GetMemberId = (from data in dbcontext.MAS_INDVSL where data.IND_Email == email && data.Status == false select data).FirstOrDefault();

                    var ForgoteData = (from data in dbcontext.MAS_UID where data.MAS_INDVSL_FID == GetMemberId.FID && data.MAS_CHC_FID == GetMemberId.MAS_CHC_FID && data.U_Status == true select data).FirstOrDefault();
                    if (newUserId != null && newPassword != null)
                    {
                        ForgoteData.U_Pass = newPassword;
                        ForgoteData.U_ID = newUserId;

                        int update = dbcontext.SaveChanges();

                        SendMail mail = new SendMail();
                        var Title= "Account Recovery Information "; 
                        var GetBody = "Dear <b>" + GetMemberId.IND_Name + "</b>,<br><br>We send  your password and UserId </br></br><br>UserID :<b>" + newUserId + " <b></br><br></br><br>Passord is <b>" + newPassword + "</b></br></br></b></br></br><br>Best regards.</br><br>Mumbai Church</br>";
                        mail.SendMailToMember(GetMemberId.IND_Email, GetBody, Title);
                       
                        if (update >= 1)
                        {
                            //TempData["Message"] = "UserId & Password change successfully";
                            TempData["Message"] = "Password send successfull ! to your register emailid";
                            TempData["Icon"] = "success";
                            return Json(new { Message = TempData["Message"], Icon = TempData["Icon"] }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            TempData["Message"] = "UserId & Password not change";
                            TempData["Icon"] = "error";
                            return Json(new { Message = TempData["Message"], Icon = TempData["Icon"] }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        TempData["Message"] = "UserId & Password not change ";
                        TempData["Icon"] = "error";
                        return Json(new { Message = TempData["Message"], Icon = TempData["Icon"] }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    TempData["Message"] = "UserId & Password not change ";
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

        #endregion




    }
}