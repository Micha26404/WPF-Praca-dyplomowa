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
		public virtual languages lang_id { get; set; }
		public virtual countries country_id { get; set; }
	}
}
