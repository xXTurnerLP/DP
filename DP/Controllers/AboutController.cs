using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DP.Controllers
{
    public class AboutController : Controller
    {
		[HttpGet]
		public IActionResult Index()
		{
			return View("Index");
		}
	}
}
