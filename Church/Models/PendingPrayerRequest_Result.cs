//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Church.Models
{
    using System;
    
    public partial class PendingPrayerRequest_Result
    {
        public Nullable<int> Req_ID { get; set; }
        public int FID { get; set; }
        public Nullable<bool> Req_Status { get; set; }
        public Nullable<System.DateTime> Req_Date { get; set; }
        public string RequestType { get; set; }
        public string PrayerRemark { get; set; }
        public string IND_Name { get; set; }
        public string AdharNo { get; set; }
        public string CHC_Name { get; set; }
    }
}
