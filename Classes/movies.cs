namespace WPF
{
	using System;
	using System.Collections.Generic;

	public partial class movies
	{
		public int id { get; set; }
		public string title { get; set; }
		public string year { get; set; }
		public int duration { get; set; }
		public Decimal price { get; set; }
		public string description { get; set; }
		public string age_min { get; set; }
		public virtual ICollection<languages> lang_id { get; set; }
		public virtual ICollection<countries> country_id { get; set; }
		public virtual ICollection<formats> format_id { get; set; }
		public virtual ICollection<roles> role_id { get; set; }
	}
}
