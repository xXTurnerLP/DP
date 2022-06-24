using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DP.Models
{
	public class Property
	{
		public string Type { get; set; } // eg. "house", "apartment", "store"
		public string State { get; set; } // "sold", "available" or "reserved"
		public bool PriceHidden { get; set; }
		public double Price { get; set; }
	}
}
