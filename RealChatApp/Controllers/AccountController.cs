using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Threading.Tasks;
using RealChatApp.Controllers;
using RealChatApp.Models;
using System.ComponentModel.DataAnnotations;
using RealChatApp.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace RealChatApp.Controllers
{

    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AccountController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(string Username, string Password, IFormFile ProfileImage)
        {
            var user = new User
            {
                Username = Username,
                Password = Password
            };

            if (ProfileImage != null)
            {
                string path = "/profileImages/" + Guid.NewGuid() + Path.GetExtension(ProfileImage.FileName);
                using var stream = new FileStream(_env.WebRootPath + path, FileMode.Create);
                await ProfileImage.CopyToAsync(stream);
                user.ProfileImagePath = path;
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("Login");
        }

        public IActionResult Login() => View();


        [HttpPost]
        public async Task<IActionResult> Login(string Username, string Password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == Username && u.Password == Password);
            if (user != null)
            {
                // Create claims
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim("ProfileImagePath", user.ProfileImagePath ?? "/default-profile.png")
        };

                // Create identity
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // Sign in
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    new AuthenticationProperties
                    {
                        IsPersistent = true // Set to true for "remember me" functionality
                    });

                HttpContext.Session.SetInt32("UserId", user.Id);
                return RedirectToAction("Index", "Chat");
            }
            ModelState.AddModelError("", "Invalid username or password");
            return View();
        }

        //[HttpPost]
        //public IActionResult Login(string Username, string Password)
        //{
        //    var user = _context.Users.FirstOrDefault(u => u.Username == Username && u.Password == Password);
        //    if (user != null)
        //    {
        //        HttpContext.Session.SetInt32("UserId", user.Id);
        //        return RedirectToAction("Index", "Chat");
        //    }
        //    ModelState.AddModelError("", "Invalid username or password");
        //    return View();
        //}

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }

}