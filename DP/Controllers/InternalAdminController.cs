using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
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

		[HttpPost]
		public IActionResult Authorize(string Password)
		{
			Debug.WriteLine(Password);
			Debug.WriteLine(Startup.RootPassword);
			Debug.WriteLine("-----");

			if (Password == Startup.RootPassword)
			{
				return View("Panel");
			}
			else
			{
				return View("WrongPassword");
			}
		}
	}
}
