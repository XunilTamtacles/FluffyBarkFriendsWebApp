using FluffyBarkFriendsWebApp.Models.Database;
using FluffyBarkFriendsWebApp.Models.ViewModels;
using FluffyBarkFriendsWebApp.Views.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FluffyBarkFriendsWebApp.Controllers
{
    [Authorize(Roles = "Admin")]
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

            var user = new User
            {
                FullName = model.FullName,
                UserName = model.UserName,
                Role = model.Role
            };

        
            user.PasswordHash = _passwordHasher
                .HashPassword(user, model.PasswordHash);

            try
            {
                await _userService.CreateAsync(user);

                TempData["Success"] = "User created successfully.";

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

            var model = new UserFormsViewModel
            {
                UserId = user.UserId,
                FullName = user.FullName,
                UserName = user.UserName,
                Role = user.Role
            };

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

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existingUser = await _userService.GetByIdAsync(id);

            if (existingUser == null)
            {
                return NotFound();
            }

            existingUser.FullName = model.FullName;
            existingUser.UserName = model.UserName;
            existingUser.Role = model.Role;

           
            if (!string.IsNullOrWhiteSpace(model.PasswordHash))
            {
                existingUser.PasswordHash = _passwordHasher
                    .HashPassword(existingUser, model.PasswordHash);
            }

            try
            {
                await _userService.UpdateAsync(existingUser);

                TempData["Success"] = "User updated successfully.";

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

            TempData["Success"] = "User deleted successfully.";

            return RedirectToAction(nameof(Index));
        }
    }
}