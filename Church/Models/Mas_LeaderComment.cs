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
    using System.Collections.Generic;
    
    public partial class Mas_LeaderComment
    {
        public int FID { get; set; }
        public Nullable<System.DateTime> FDate { get; set; }
        public string Mac_ID { get; set; }
        public string Mac_IP { get; set; }
        public Nullable<int> LeaderFID { get; set; }
        public string Comments { get; set; }
        public Nullable<int> ReqFId { get; set; }
    }
}
