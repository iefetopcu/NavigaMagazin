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
    
    public partial class magazineTable
    {
        public long id { get; set; }
        public string coverphoto { get; set; }
        public string magazinetitle { get; set; }
        public string magazinecontent { get; set; }
        public Nullable<System.DateTime> magazinetime { get; set; }
        public Nullable<System.DateTime> creationtime { get; set; }
        public string url { get; set; }
    }
}
