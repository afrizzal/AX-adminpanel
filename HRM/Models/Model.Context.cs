﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HRM.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class HRMEntities : DbContext
    {
        public HRMEntities()
            : base("name=HRMEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<BENEFIT> BENEFIT { get; set; }
        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<District> Districts { get; set; }
        public virtual DbSet<FAQ> FAQ { get; set; }
        public virtual DbSet<HighlightKatalogues> HighlightKatalogues { get; set; }
        public virtual DbSet<HighlightNews> HighlightNews { get; set; }
        public virtual DbSet<HighlightPromotion> HighlightPromotion { get; set; }
        public virtual DbSet<HighlightStore> HighlightStores { get; set; }
        public virtual DbSet<Katalogue> Katalogues { get; set; }
        public virtual DbSet<Media> Media { get; set; }
        public virtual DbSet<News> News { get; set; }
        public virtual DbSet<Promo> Promoes { get; set; }
        public virtual DbSet<Province> Provinces { get; set; }
        public virtual DbSet<Store> Stores { get; set; }
    }
}
