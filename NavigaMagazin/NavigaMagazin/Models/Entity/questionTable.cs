//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NavigaMagazin.Models.Entity
{
    using System;
    using System.Collections.Generic;
    
    public partial class questionTable
    {
        public long id { get; set; }
        public Nullable<long> userid { get; set; }
        public Nullable<long> expertid { get; set; }
        public string questiontitle { get; set; }
        public string questioncontent { get; set; }
        public string questionanswer { get; set; }
        public Nullable<System.DateTime> questiontime { get; set; }
        public Nullable<int> isaktif { get; set; }
    
        public virtual UserTable UserTable { get; set; }
        public virtual UserTable UserTable1 { get; set; }
    }
}
