using DP.Models;
using DP.Models.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
			dbCache.offers = database.Offers.ToList();
		}

		[HttpGet]
		public IActionResult Index()
		{
			return View("Index", dbCache);
		}

		[HttpGet]
		public IActionResult CreateOffer()
		{
			return View("Create", dbCache);
		}

		[HttpPost]
		public void CreateOffer(float price, string city, string street, string description, string state, IFormFile img)
		{
			string sessionId = Request.Cookies["SessionId"];
			if (sessionId == string.Empty)
			{
				Response.Redirect("/Offers");
			}

			var session = database.Sessions.FirstOrDefault(s => s.SessionId == sessionId);

			if (session == null || session.Account.Role != "Admin")
			{
				Response.Redirect("/Offers");
			}

			var stream = img.OpenReadStream();
			byte[] bytes = new byte[stream.Length];
			stream.Read(bytes, 0, (int)stream.Length);

			Offer offer = new Offer()
			{
				Price = price,
				City = city,
				Street = street,
				Description = description,
				State = state,
				Base64Img = Convert.ToBase64String(bytes)
			};

			database.Offers.Add(offer);
			database.SaveChanges();

			Response.Redirect("/Offers");
		}

		[HttpPost]
		public void DeleteOffer(int id)
		{
			string sessionId = Request.Cookies["SessionId"];
			if (sessionId == string.Empty)
			{
				Response.Redirect("/Offers");
			}

			var session = database.Sessions.FirstOrDefault(s => s.SessionId == sessionId);

			if (session == null || session.Account.Role != "Admin")
			{
				Response.Redirect("/Offers");
			}

			var offer = database.Offers.FirstOrDefault(o => o.Id == id);
			if (offer != null)
			{
				database.Offers.Remove(offer);
				database.SaveChanges();
			}

			Response.Redirect("/Offers");
		}

		[HttpPost]
		public IActionResult EditOffer(int id)
		{
			string sessionId = Request.Cookies["SessionId"];
			if (sessionId == string.Empty)
			{
				Response.Redirect("/Offers");
			}

			var session = database.Sessions.FirstOrDefault(s => s.SessionId == sessionId);

			if (session == null || session.Account.Role != "Admin")
			{
				Response.Redirect("/Offers");
			}

			var offer = database.Offers.FirstOrDefault(o => o.Id == id);
			if (offer != null)
			{
				ViewData["offer"] = offer;
				return View("Edit", dbCache);
			}

			return View("Index", dbCache);
		}

		[HttpPost]
		public void EditOffer_(int id, float price, string city, string street, string description, string state, IFormFile img)
		{
			string sessionId = Request.Cookies["SessionId"];
			if (sessionId == string.Empty)
			{
				Response.Redirect("/Offers");
			}

			var session = database.Sessions.FirstOrDefault(s => s.SessionId == sessionId);

			if (session == null || session.Account.Role != "Admin")
			{
				Response.Redirect("/Offers");
			}

			var offer = database.Offers.FirstOrDefault(o => o.Id == id);
			if (offer != null)
			{
				offer.Price = price;
				offer.City = city;
				offer.Street = street;
				offer.Description = description;
				offer.State = state;
				
				if (img != null)
				{
					var stream = img.OpenReadStream();
					byte[] bytes = new byte[stream.Length];
					stream.Read(bytes, 0, (int)stream.Length);

					offer.Base64Img = Convert.ToBase64String(bytes);
				}

				database.SaveChanges();
			}

			Response.Redirect("/Offers");
		}
	}
}
