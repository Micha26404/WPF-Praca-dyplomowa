namespace WPF
{
    using System;
    using System.Collections.Generic;
    
    public partial class clients
    {
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public clients()
        {
            this.order_id = new HashSet<orders>();
        }
        public int id { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
    
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<orders> order_id { get; set; }
    }
}
