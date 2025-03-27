using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using P50_4_22.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace P50_4_22.Controllers
{
    public class ProfileController : Controller
    {
        private readonly PetStoreRpmContext db;
        private readonly IMemoryCache _memoryCache;
        private readonly Dictionary<string, string> _resetCodes = new Dictionary<string, string>(); // Упрощаем: храним только email и код

        public ProfileController(PetStoreRpmContext context, IMemoryCache memoryCache)
        {
            db = context;
            _memoryCache = memoryCache; 
        }

        [HttpGet]
        public IActionResult Profile()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("UserProfile");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Profile(string Email, string Loginpassword)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("UserProfile");
            }

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

                return RedirectToAction("UserProfile");
            }

            ViewBag.ErrorMessage = "Неверные учетные данные";
            return View();
        }

        [HttpGet]
        public IActionResult Registr()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("UserProfile");
            }
            return RedirectToAction("Profile");
        }

        [HttpPost]
        public async Task<IActionResult> Registr(string Loginvhod, string Loginpassword, string PhoneNumber, string Email, string ClientName)
        {
            if (db.Users.Any(u => u.Email == Email))
            {
                ViewBag.ErrorMessage = "Пользователь с таким email уже существует";
                return View("Profile");
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

            return RedirectToAction("Profile");
        }

        [HttpGet]
        [Authorize]
        public IActionResult UserProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Profile");
            }

            var user = db.Users.FirstOrDefault(u => u.IdUsers == int.Parse(userId));
            if (user == null)
            {
                return RedirectToAction("Profile");
            }

            return View(user);
        }

        [HttpPost]
        public IActionResult ForgotPassword([FromBody] ForgotPasswordModel model)
        {
            var user = db.Users.FirstOrDefault(u => u.Email == model.Email);
            if (user == null)
            {
                return Json(new { success = false, message = "Пользователь с таким email не найден" });
            }

            string code = new Random().Next(1000, 9999).ToString();
            string emailLower = model.Email.ToLower();

            // Сохраняем код в кэш на 10 минут
            _memoryCache.Set(emailLower, code, TimeSpan.FromMinutes(10));

            Console.WriteLine($"ForgotPassword: Email = {emailLower}, Code = {code}");

            try
            {
                SendResetCodeEmail(model.Email, code);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Ошибка при отправке email: " + ex.Message });
            }
        }


        [HttpPost]
        public IActionResult VerifyCode([FromBody] VerifyCodeModel model)
        {
            string emailLower = model.Email.ToLower();

            if (!_memoryCache.TryGetValue(emailLower, out string storedCode))
            {
                return Json(new { success = false, message = "Код не был отправлен" });
            }

            if (storedCode != model.Code)
            {
                return Json(new { success = false, message = "Неверный код" });
            }

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        {
            string emailLower = model.Email.ToLower();

            if (!_memoryCache.TryGetValue(emailLower, out string _))
            {
                return Json(new { success = false, message = "Сессия восстановления пароля истекла" });
            }

            var user = db.Users.FirstOrDefault(u => u.Email == model.Email);
            if (user == null)
            {
                return Json(new { success = false, message = "Пользователь не найден" });
            }

            user.Loginpassword = HashPassword(model.NewPassword);
            db.Users.Update(user);
            await db.SaveChangesAsync();

            _memoryCache.Remove(emailLower);

            return Json(new { success = true });
        }

        private void SendResetCodeEmail(string email, string code)
        {
            using (var client = new SmtpClient("smtp.mail.ru", 587))
            {
                client.EnableSsl = true;
                client.Credentials = new System.Net.NetworkCredential("tema.murashov.06@mail.ru", "8SRBSE15bWDXnYX01iYc");

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("tema.murashov.06@mail.ru"),
                    Subject = "Код для восстановления пароля",
                    Body = $"Ваш код для восстановления пароля: {code}",
                    IsBodyHtml = false,
                };
                mailMessage.To.Add(email);

                client.Send(mailMessage);
            }
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
            return RedirectToAction("Profile");
        }
    }

    public class ForgotPasswordModel
    {
        public string Email { get; set; }
    }

    public class VerifyCodeModel
    {
        public string Email { get; set; }
        public string Code { get; set; }
    }

    public class ResetPasswordModel
    {
        public string Email { get; set; }
        public string NewPassword { get; set; }
    }
}