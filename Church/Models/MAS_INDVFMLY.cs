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
    
    public partial class MAS_INDVFMLY
    {
        public int Fid { get; set; }
        public Nullable<System.DateTime> Fdate { get; set; }
        public string Mid { get; set; }
        public string Mip { get; set; }
        public Nullable<int> MAS_INDVSL_Fid { get; set; }
        public string Relation { get; set; }
        public Nullable<int> Member_id { get; set; }
        public Nullable<bool> Active { get; set; }
    }
}
