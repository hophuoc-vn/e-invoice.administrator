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
    
    public partial class ServicePackage
    {
        public int ID { get; set; }
        public Nullable<int> ComID { get; set; }
        public string invPattern { get; set; }
        public string invSerial { get; set; }
        public string ServiceCode { get; set; }
        public string ServiceName { get; set; }
        public Nullable<int> TotalInvoice { get; set; }
        public Nullable<int> Status { get; set; }
        public string ServiceID { get; set; }
        public Nullable<int> IsActive { get; set; }
    }
}