//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ManageHDDT.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class dhql_user
    {
        public int id { get; set; }
        public string name { get; set; }
        public string email_address { get; set; }
        public string login_token { get; set; }
        public string role { get; set; }
        public string functions { get; set; }
        public Nullable<System.DateTime> create_date { get; set; }
        public Nullable<System.DateTime> modify_date { get; set; }
        public Nullable<bool> flag { get; set; }
    }
}