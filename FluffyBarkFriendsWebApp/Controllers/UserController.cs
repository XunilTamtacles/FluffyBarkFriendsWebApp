using FluffyBarkFriendsWebApp.Models.Database;
using FluffyBarkFriendsWebApp.Models.ViewModels;
using FluffyBarkFriendsWebApp.Views.Service.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FluffyBarkFriendsWebApp.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly PasswordHasher<User> _passwordHasher;

        public UserController(IUserService userService)
        {
            _userService = userService;
            _passwordHasher = new PasswordHasher<User>();
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllAsync();
            return View(users);
        }

        public async Task<IActionResult> Details(int id)
        {
            var user = await _userService.GetByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        public IActionResult Create()
        {
            return View(new UserFormsViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserFormsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = MapToUser(model);
            user.PasswordHash = _passwordHasher.HashPassword(user, model.PasswordHash);

            try
            {
                await _userService.CreateAsync(user);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userService.GetByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var model = MapToUserFormsViewModel(user);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserFormsViewModel model)
        {
            if (id != model.UserId)
            {
                return NotFound();
            }

            var existingUser = await _userService.GetByIdAsync(id);

            if (existingUser == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = MapToUser(model);
            user.PasswordHash = _passwordHasher.HashPassword(user, model.PasswordHash);

            try
            {
                await _userService.UpdateAsync(user);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userService.GetByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _userService.GetByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            await _userService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private static User MapToUser(UserFormsViewModel model)
        {
            return new User
            {
                UserId = model.UserId,
                FullName = model.FullName,
                UserName = model.UserName,
                PasswordHash = model.PasswordHash,
                Role = model.Role
            };
        }

        private static UserFormsViewModel MapToUserFormsViewModel(User user)
        {
            return new UserFormsViewModel
            {
                UserId = user.UserId,
                FullName = user.FullName,
                UserName = user.UserName,
                Role = user.Role
            };
        }
    }
}
