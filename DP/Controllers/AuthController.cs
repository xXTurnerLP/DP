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
using Microsoft.AspNetCore.Http;
using System.Web;

namespace DP.Controllers
{
	public class AuthController : Controller
	{
		const string salt = "salt1337ASJKHD";

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
			return View("RootLogin");
		}

		[HttpGet]
		public IActionResult Login()
		{
			return View("NormalUser/Login");
		}

		[HttpGet]
		public IActionResult Register()
		{
			return View("NormalUser/Register");
		}

		[HttpPost]
		public IActionResult Login(string email, string password, string remember)
		{
			bool doRemember = remember == "on";

			User user = database.Users.FirstOrDefault(u => u.Email == email);

			if (user == null)
			{
				ViewData["message"] = "Не съществува акаунт с такъв имейл адрес!";
				ViewData["view"] = "NormalUser/Login";
				return View("Error");
			}
			else
			{
				if (user.Password != sha256_hash($"{email}{salt}{password}"))
				{
					ViewData["message"] = "Неправилна парола!";
					ViewData["view"] = "NormalUser/Login";
					return View("Error");
				}
				else
				{
					string sessionId = Request.Cookies["SessionId"];
					string newSessionId = Guid.NewGuid().ToString();

					CookieOptions cookieOptions = new CookieOptions
					{
						IsEssential = true
					};

					if (doRemember)
						cookieOptions.Expires = DateTime.Now.AddDays(365);

					if (sessionId != string.Empty)
					{
						Session oldSession = database.Sessions.FirstOrDefault(s => s.SessionId == sessionId);

						if (oldSession != null)
							database.Sessions.Remove(oldSession);
					}

					database.Sessions.Add(new Session { SessionId = newSessionId, Account = user });
					database.SaveChangesAsync();

					Response.Cookies.Delete("SessionId");
					Response.Cookies.Append("SessionId", newSessionId, cookieOptions);
					return Redirect("/");
				}
			}
		}

		[HttpPost]
		public IActionResult Register(string email, string password, string repeatPassword, string remember)
		{
			bool doRemember = remember == "on";

			if (database.Users.Any(u => u.Email == email))
			{
				ViewData["message"] = "Вече съществува акаунт с този имейл адрес!";
				ViewData["view"] = "NormalUser/Register";
				return View("Error");
			}

			if (password != repeatPassword)
			{
				ViewData["message"] = "Паролите не съвпадат!";
				ViewData["view"] = "NormalUser/Register";
				return View("Error");
			}

			User user = new User
			{
				Email = email,
				Password = sha256_hash($"{email}{salt}{password}"),
				Role = "User"
			};

			database.Users.Add(user);

			string sessionId = Request.Cookies["SessionId"];
			string newSessionId = Guid.NewGuid().ToString();

			CookieOptions cookieOptions = new CookieOptions
			{
				IsEssential = true
			};

			if (doRemember)
				cookieOptions.Expires = DateTime.Now.AddDays(365);

			if (sessionId != string.Empty)
			{
				Session oldSession = database.Sessions.FirstOrDefault(s => s.SessionId == sessionId);

				if (oldSession != null)
					database.Sessions.Remove(oldSession);
			}

			database.Sessions.Add(new Session { SessionId = newSessionId, Account = user });
			database.SaveChangesAsync();

			Response.Cookies.Delete("SessionId");
			Response.Cookies.Append("SessionId", newSessionId, cookieOptions);

			return Redirect("/");
		}

		[HttpPost]
		public IActionResult RootLogin(string rootPassword)
		{
			if (rootPassword == m_RootPassword)
			{
				ViewData["rootPassword"] = rootPassword;
				return View("Panel");
			}
			else
			{
				ViewData["message"] = "Невалидна парола!";
				ViewData["view"] = "RootLogin";
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
				ViewData["view"] = "RootLogin";
				return View("Error");
			}

			bool isAdminBool = isAdmin == "on";

			if (password != repeatPassword)
			{
				ViewData["message"] = "Паролите не съвпадат!";
				ViewData["view"] = "Panel";
				return View("Error");
			}

			if (database.Users.Where(u => u.Email == email).ToArray().Length != 0)
			{
				ViewData["message"] = "Вече съществува акаунт с този имейл адрес!";
				ViewData["view"] = "Panel";
				return View("Error");
			}

			SHA256 sha256 = SHA256.Create();

			User user = new User
			{
				Email = email,
				Password = sha256_hash($"{email}{salt}{password}"),
				Role = isAdminBool ? "Admin" : "User"
			};

			database.Users.Add(user);
			database.SaveChangesAsync();

			ViewData["message"] = isAdminBool ? "Успешно създаден администраторски акаунт!" : "Успешно създаден акаунт!";
			ViewData["view"] = "Panel";
			return View("Success");
		}
	}
}
