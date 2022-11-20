namespace WPF
{
    using System;
    using System.Collections.Generic;
    
    public partial class orders
    {
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public int id { get; set; }
		public virtual ICollection<movies> movie_id { get; set; }
		public byte quantity { get; set; }
        public System.DateTime rent_date { get; set; }
        public System.DateTime due_date { get; set; }
		public System.DateTime return_date { get; set; }
		public virtual clients client_id { get; set; }
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
    }
}
