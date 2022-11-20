namespace WPF
{
    using System;
    using System.Collections.Generic;
    
    public partial class languages
    {
        public int id { get; set; }
        public string name { get; set; }
        public virtual movies movie { get; set; }
    }
}
