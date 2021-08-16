//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class Store
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Store()
        {
            this.HighlightPromotions = new HashSet<HighlightPromotion>();
            this.News = new HashSet<News>();
            this.Promoes = new HashSet<Promo>();
        }
    
        public decimal ID { get; set; }
        public string NAME { get; set; }
        public string PHONE { get; set; }
        public string WHATSAPP { get; set; }
        public string WORK_HOUR { get; set; }
        public string GOOGLE_ADDRESS { get; set; }
        public string LATITUDE { get; set; }
        public string LOGITUDE { get; set; }
        public string IMAGE_URL { get; set; }
        public string STREET { get; set; }
        public Nullable<decimal> ID_PROVINCE { get; set; }
        public Nullable<decimal> ID_CITY { get; set; }
        public Nullable<decimal> ID_DISTRICT { get; set; }
        public string ZIPCODE { get; set; }
        public string FASILITAS { get; set; }
        public System.DateTime CREATED_AT { get; set; }
        public System.DateTime UPDATED_AT { get; set; }
    
        public virtual City City { get; set; }
        public virtual District District { get; set; }
        public virtual Province Province { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HighlightPromotion> HighlightPromotions { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<News> News { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Promo> Promoes { get; set; }
    }
}
