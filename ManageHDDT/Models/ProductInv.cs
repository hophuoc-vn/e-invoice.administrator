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
    
    public partial class ProductInv
    {
        public System.Guid id { get; set; }
        public Nullable<int> InvID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Remark { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<double> Quantity { get; set; }
        public string Unit { get; set; }
        public Nullable<decimal> Total { get; set; }
        public Nullable<double> VATRate { get; set; }
        public Nullable<decimal> Discount { get; set; }
        public Nullable<decimal> VATAmount { get; set; }
        public Nullable<decimal> DiscountAmount { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<int> ProdType { get; set; }
        public Nullable<int> IsSum { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string Extra { get; set; }
        public Nullable<int> ComID { get; set; }
        public Nullable<int> Stt { get; set; }
        public string ConNo { get; set; }
        public string ExpDate { get; set; }
        public Nullable<decimal> TotalChuaCK { get; set; }
    }
}
