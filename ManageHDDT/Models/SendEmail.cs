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
    
    public partial class SendEmail
    {
        public System.Guid id { get; set; }
        public string Email { get; set; }
        public string CCEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string EmailFrom { get; set; }
        public string FileName { get; set; }
        public byte[] FileAttach { get; set; }
        public string GroupName { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<System.DateTime> SendedDate { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string Note { get; set; }
        public string InvToken { get; set; }
        public string BodySms { get; set; }
        public string SmsTitle { get; set; }
        public Nullable<int> SmsStatus { get; set; }
        public Nullable<int> InvId { get; set; }
        public Nullable<int> Type { get; set; }
    }
}
