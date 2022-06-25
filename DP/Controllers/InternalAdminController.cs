using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DP.Controllers
{
	public class InternalAdminController : Controller
	{
		public IActionResult Index()
		{
			return View("PasswordProtected");
		}

		public IActionResult Authorize()
		{
			return View();
		}
	}
}
