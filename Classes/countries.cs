namespace WPF
{
    using System;
    using System.Collections.Generic;
    
    public partial class countries
    {
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public countries()
        {
            this.langs = new HashSet<languages>();
        }
        public int country_id { get; set; }
        public string name { get; set; }
    
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<languages> langs { get; set; }
    }
}
