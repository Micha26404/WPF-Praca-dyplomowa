//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WPF.database
{
    using System;
    using System.Collections.Generic;
    
    public partial class orders
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public orders()
        {
            this.quantity = 1;
            this.movies = new HashSet<movies>();
        }
    
        public int id { get; set; }
        public byte quantity { get; set; }
        public System.DateTime rent_date { get; set; }
        public System.DateTime due_date { get; set; }
        public System.DateTime return_date { get; set; }
    
        public virtual clients client { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<movies> movies { get; set; }
    }
}
