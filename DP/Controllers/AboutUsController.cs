using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DP.Controllers
{
	public class AboutUsController : Controller
	{
		public IActionResult AboutUs()
		{
			return View();
		}
	}
}
