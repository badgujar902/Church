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
    
    public partial class sp_List_Notice_Result
    {
        public int FId { get; set; }
        public Nullable<System.DateTime> FDate { get; set; }
        public Nullable<int> LeaderFId { get; set; }
        public Nullable<int> CurchId { get; set; }
        public string NoticeDate { get; set; }
        public string NoticeSubject { get; set; }
        public string NoticeDescription { get; set; }
        public string NoticeDateValid { get; set; }
        public Nullable<System.DateTime> NoticeTillDate { get; set; }
        public Nullable<bool> Meal_Attandence { get; set; }
        public string CHC_Name { get; set; }
    }
}
