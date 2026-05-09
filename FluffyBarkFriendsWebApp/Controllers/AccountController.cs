using System.Security.Claims;
using FluffyBarkFriendsWebApp.Models.Database;
using FluffyBarkFriendsWebApp.Models.ViewModels;
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
            return View(new LoginViewModel
            {
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userService.GetByUsernameAsync(model.Username.Trim());

            if (user == null || !CheckPassword(user, model.Password))
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                return View(model);
            }

            var role = NormalizeRole(user.Role);

            if (role != "Admin" && role != "Staff" && role != "Client")
            {
                ModelState.AddModelError(string.Empty, "This account role is not allowed to login.");
                return View(model);
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

            if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            if (role == "Client")
            {
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index", "DashBoard");
        }

        public IActionResult Register()
        {
            return View(new RegisterViewModel
            {
                Role = "Client"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var role = NormalizeRole(model.Role);

            if (role != "Admin" && role != "Staff" && role != "Client")
            {
                ModelState.AddModelError(nameof(model.Role), "Role must be Admin, Staff, or Client.");
                return View(model);
            }

            var user = new User
            {
                FullName = model.FullName.Trim(),
                UserName = model.Username.Trim(),
                Role = role,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);

            try
            {
                await _userService.CreateAsync(user);
                return RedirectToAction(nameof(Login));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login), "Account");
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

            if (role.Equals("client", StringComparison.OrdinalIgnoreCase))
            {
                return "Client";
            }

            return role;
        }
    }
}