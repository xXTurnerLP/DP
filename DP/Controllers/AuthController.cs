using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using DP.Models.Auth.Contexts;
using Microsoft.EntityFrameworkCore;
using DP.Models.Auth;
using System.Collections.ObjectModel;
using System.Security.Cryptography;

namespace DP.Controllers
{
	public class AuthController : Controller
	{
		readonly UserContext userDb;
		readonly string m_RootPassword;

		public AuthController(DbContextOptions options)
		{
			userDb = new UserContext(options);

			IConfigurationRoot configuration = new ConfigurationBuilder()
				.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
				.AddJsonFile("appsettings.json")
				.Build();

			m_RootPassword = configuration.GetValue<string>("RootPassword");
		}

		[HttpGet]
		public IActionResult RootLogin()
		{
			return View("RootLoginPartial");
		}

		[HttpPost]
		public IActionResult RootLogin(string rootPassword)
		{
			if (rootPassword == m_RootPassword)
			{
				return View("PanelPartial");
			}
			else
			{
				ViewData["message"] = "Невалидна парола!";
				ViewData["view"] = "RootLoginPartial";
				return View("Error");
			}
		}

		static String sha256_hash(String value)
		{
			StringBuilder Sb = new StringBuilder();

			using (SHA256 hash = SHA256Managed.Create())
			{
				Encoding enc = Encoding.UTF8;
				Byte[] result = hash.ComputeHash(enc.GetBytes(value));

				foreach (Byte b in result)
					Sb.Append(b.ToString("x2"));
			}

			return Sb.ToString();
		}

		[HttpPost]
		public IActionResult CreateAccount(string email, string password, string repeatPassword, bool isAdmin)
		{
			if (password != repeatPassword)
			{
				ViewData["message"] = "Паролите не съвпадат!";
				ViewData["view"] = "PanelPartial";
				return View("Error");
			}

			if (userDb.User.Where(u => u.Email == email).ToArray().Length != 0)
			{
				ViewData["message"] = "Вече съществува акаунт с този имейл адрес!";
				ViewData["view"] = "PanelPartial";
				return View("Error");
			}

			SHA256 sha256 = SHA256.Create();

			User user = new User
			{
				Email = email,
				Password = sha256_hash($"salt1337ASJKHD{password}"),
				Role = isAdmin ? "Admin" : "User"
			};

			userDb.User.Add(user);
			userDb.SaveChangesAsync();

			ViewData["message"] = isAdmin ? "Успешно създаден администраторски акаунт!" : "Успешно създаден акаунт!";
			ViewData["view"] = "PanelPartial";
			return View("Success");
		}
	}
}
