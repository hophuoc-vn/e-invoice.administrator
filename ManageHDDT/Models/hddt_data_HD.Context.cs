﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class hddt_data_HD_Entities : DbContext
    {
        public hddt_data_HD_Entities()
            : base("name=hddt_data_HD_Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Decision> Decisions { get; set; }
        public virtual DbSet<MailTemplate> MailTemplates { get; set; }
        public virtual DbSet<Publish> Publishes { get; set; }
        public virtual DbSet<PublishInvoice> PublishInvoices { get; set; }
        public virtual DbSet<SendEmail> SendEmails { get; set; }
        public virtual DbSet<userdata> userdatas { get; set; }
        public virtual DbSet<VATInvoice> VATInvoices { get; set; }
        public virtual DbSet<ProductInv> ProductInvs { get; set; }
    }
}
