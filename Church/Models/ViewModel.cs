using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace Church.Models
{
    public class ViewModel
    {
        public MAS_LEADER MAS_LEADER { get; set; }
        public MAS_CHC MAS_CHC { get; set; }
        public MAS_UID MAS_UID { get; set; }
        public MAS_INDVSL MAS_INDVSL { get; set; }
        public MAS_ACTIVITY_I MAS_ACTIVITY_I { get; set; }
        public MAS_ACTIVITY_II MAS_ACTIVITY_II { get; set; }
        public MAS_Calender MAS_Calender { get; set; }
        public MAS_PrayerReq MAS_PrayerReq { get; set; }


    }


    public class Individual
    {
        public int Fid { get; set; }
        public int MAS_CHC_FID { get; set; }
        public string MemberAdharNo { get; set; }
        public string IND_Name { get; set; }
        public string IND_Mob { get; set; }
        public string IND_Email { get; set; }
        public string IND_Address { get; set; }
        public DateTime IND_DOJ { get; set; }
        public DateTime IND_DOB { get; set; }
        public string file { get; set; }
        public string IND_ReffName { get; set; }
        public string U_ID { get; set; }
        public string U_Pass { get; set; }
        public string Reletion { get; set; }
        public string Gender { get; set; }
        public int StateFid { get; set; }
        public string City { get; set; }
        public string Area { get; set; }


    }

    public class EventCreate
    {
        public int Fid { get; set; }
        public int MAS_CHC_FID { get; set; }
        public string EventNote { get; set; }
        public string ImageURL { get; set; }
    }


    public class NotificationModel
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public NotificationData Data { get; set; }
    }

    public class NotificationData
    {
        public string ScreenName { get; set; }
        public string Type { get; set; }
    }

    public class ChurchCreate
    {
        public int Fid { get; set; }
        public string ChurchName { get; set; }
        public int Mas_StateId { get; set; }
        public string Mas_City { get; set; }
        public string Area { get; set; }
        public string PinCode { get; set; }
        public string ChurchAddress { get; set; }
    }  

}