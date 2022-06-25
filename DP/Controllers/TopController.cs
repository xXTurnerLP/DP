using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DP.Controllers
{
    public class TopController : Controller
    {
		[HttpGet]
		public IActionResult Index()
		{
			return View("Index");
		}
	}
}
