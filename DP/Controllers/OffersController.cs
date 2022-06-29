using DP.Models;
using DP.Models.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DP.Controllers
{
    public class OffersController : Controller
    {
		readonly ApplicationContext database;
		readonly DbCache dbCache;

		public OffersController(DbContextOptions options)
		{
			database = new ApplicationContext(options);
			dbCache = new DbCache();
			dbCache.sessions = database.Sessions.ToList();
			dbCache.users = database.Users.ToList();
		}

		[HttpGet]
		public IActionResult Index()
		{
			return View("Index", dbCache);
		}
	}
}
