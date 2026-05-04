using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using FluffyBarkFriendsWebApp.Models.Database;
using FluffyBarkFriendsWebApp.Views.Repositories.Interface;
using FluffyBarkFriendsWebApp.Models.ViewModels;
using FluffyBarkFriendsWebApp.Views.Services.Interface;

namespace FluffyBarkFriendsWebApp.Controllers
{
    public class AppointmenController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
