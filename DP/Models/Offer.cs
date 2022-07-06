using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DP.Models
{
    public class Offer
    {
		public int Id { get; set; }
		public float Price { get; set; }
		public string City { get; set; }
		public string Street { get; set; }
		public string Description { get; set; }
		public string State { get; set; }
		public string Base64Img { get; set; }
	}
}
