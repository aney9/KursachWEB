using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using P50_4_22.Models;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace P50_4_22.Controllers
{
	public class ProfileController : Controller
	{

		private readonly PetStoreRpmContext db;

		public ProfileController(PetStoreRpmContext context)
		{
			db = context;
		}

		[HttpGet]
		public IActionResult Profile()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Profile(string Email, string Loginpassword)
		{
			string hashedPassword = HashPassword(Loginpassword);
			var user = db.Users.FirstOrDefault(u => u.Email == Email && u.Loginpassword == hashedPassword);

			if (user != null)
			{
				var claims = new List<Claim>
				{
					new Claim(ClaimTypes.Name, user.ClientName),
					new Claim(ClaimTypes.Email, user.Email),
					new Claim(ClaimTypes.NameIdentifier, user.IdUsers.ToString()),
					new Claim(ClaimTypes.Role, user.RolesId == 2 ? "пользователь" : "админ")
				};

				var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

				var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

				await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

				return RedirectToAction("Catalog", "User");
			}
			ViewBag.ErrorMessage = "Неверные учетные данные";
			return View();
		}

		[HttpGet]
		public IActionResult Registr()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Registr(string Loginvhod, string Loginpassword, string PhoneNumber, string Email, string ClientName)
		{
			if (db.Users.Any(u => u.Email == Email))
			{
				ViewBag.ErrorMessage = "Пользователь с таким email уже существует";
				return View();
			}

			string hashedPassword = HashPassword(Loginpassword);

			var user = new User
			{
				Loginvhod = Loginvhod,
				Loginpassword = hashedPassword,
				PhoneNumber = PhoneNumber,
				Email = Email,
				ClientName = ClientName, 
				RolesId = 2
            };
			db.Users.Add(user);
			await db.SaveChangesAsync();

			return RedirectToAction("Profile", "Profile");

		}

		private string HashPassword(string password)
		{
			using (var sha256 = SHA256.Create())
			{
				var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
				return BitConverter.ToString(hashedBytes).Replace("-", "-").ToLower();
			}
		}

		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return RedirectToAction("Profile", "Profile");
		}



	}
}
