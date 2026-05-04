using System.Security.Claims;
using FluffyBarkFriendsWebApp.Models.Database;
using FluffyBarkFriendsWebApp.Views.Service.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FluffyBarkFriendsWebApp.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly PasswordHasher<User> _passwordHasher;

        public AccountController(IUserService userService)
        {
            _userService = userService;
            _passwordHasher = new PasswordHasher<User>();
        }

        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password, string? returnUrl = null)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Username and password are required.";
                ViewBag.ReturnUrl = returnUrl;
                return View();
            }

            var user = await _userService.GetByUsernameAsync(username.Trim());

            if (user == null || !CheckPassword(user, password))
            {
                ViewBag.Error = "Invalid username or password.";
                ViewBag.ReturnUrl = returnUrl;
                return View();
            }

            var role = NormalizeRole(user.Role);

            if (role != "Admin" && role != "Staff")
            {
                ViewBag.Error = "Only Admin and Staff accounts can login.";
                ViewBag.ReturnUrl = returnUrl;
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Role, role)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account", "Appointment");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        private bool CheckPassword(User user, string password)
        {
            try
            {
                var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

                if (result == PasswordVerificationResult.Success ||
                    result == PasswordVerificationResult.SuccessRehashNeeded)
                {
                    return true;
                }
            }
            catch
            {
                // For old records that are not hashed yet
            }

            return user.PasswordHash == password;
        }

        private static string NormalizeRole(string role)
        {
            if (string.IsNullOrWhiteSpace(role))
            {
                return string.Empty;
            }

            role = role.Trim();

            if (role.Equals("admin", StringComparison.OrdinalIgnoreCase))
            {
                return "Admin";
            }

            if (role.Equals("staff", StringComparison.OrdinalIgnoreCase))
            {
                return "Staff";
            }

            return role;
        }
    }
}