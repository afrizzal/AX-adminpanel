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
    
    public partial class HighlightPromotion
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public HighlightPromotion()
        {
            this.Promoes = new HashSet<Promo>();
            this.Stores = new HashSet<Store>();
        }
    
        public decimal ID { get; set; }
        public string TITLE { get; set; }
        public string SUBTITLE { get; set; }
        public string IMAGE { get; set; }
        public System.DateTime CREATED_AT { get; set; }
        public System.DateTime UPDATED_AT { get; set; }
        public string CONTEN { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Promo> Promoes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Store> Stores { get; set; }
    }
}
