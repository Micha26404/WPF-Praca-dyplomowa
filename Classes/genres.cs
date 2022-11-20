namespace WPF
{
    using System;
    using System.Collections.Generic;
    
    public partial class genres
    {
        public int genre_id { get; set; }
        public string name { get; set; }
        public virtual movies movie { get; set; }
    }
}
