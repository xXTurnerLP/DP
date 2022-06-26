using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using DP.Models.Auth;
using System.Collections.ObjectModel;
using System.Security.Cryptography;
using DP.Models;
using System;

namespace DP.Controllers
{
	public class AuthController : Controller
	{
		readonly ApplicationContext database;
		readonly string m_RootPassword;

		public AuthController(DbContextOptions options)
		{
			database = new ApplicationContext(options);

			IConfigurationRoot configuration = new ConfigurationBuilder()
				.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
				.AddJsonFile("appsettings.json")
				.Build();

			m_RootPassword = configuration.GetValue<string>("RootPassword");
		}

		~AuthController()
		{
			database.Dispose();
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
				ViewData["rootPassword"] = rootPassword;
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
		public IActionResult RootCreateAccount(string email, string password, string repeatPassword, string isAdmin, string rootPassword)
		{
			if (rootPassword != m_RootPassword)
			{
				ViewData["message"] = "Невалидна парола!";
				ViewData["view"] = "RootLoginPartial";
				return View("Error");
			}

			bool isAdminBool = isAdmin == "on";

			if (password != repeatPassword)
			{
				ViewData["message"] = "Паролите не съвпадат!";
				ViewData["view"] = "PanelPartial";
				return View("Error");
			}

			if (database.Users.Where(u => u.Email == email).ToArray().Length != 0)
			{
				ViewData["message"] = "Вече съществува акаунт с този имейл адрес!";
				ViewData["view"] = "PanelPartial";
				return View("Error");
			}

			SHA256 sha256 = SHA256.Create();

			User user = new User
			{
				Email = email,
				Password = sha256_hash($"{email}salt1337ASJKHD{password}"),
				Role = isAdminBool ? "Admin" : "User"
			};

			database.Users.Add(user);
			database.SaveChangesAsync();

			ViewData["message"] = isAdminBool ? "Успешно създаден администраторски акаунт!" : "Успешно създаден акаунт!";
			ViewData["view"] = "PanelPartial";
			return View("Success");
		}
	}
}
