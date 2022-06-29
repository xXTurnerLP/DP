using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DP.Models;
using DP.Models.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DP.Controllers
{
    public class HomeController : Controller
    {
		readonly ApplicationContext database;
		readonly DbCache dbCache;

		public HomeController(DbContextOptions options)
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
