using Microsoft.AspNetCore.Mvc;

namespace FluffyBarkFriendsWebApp.Controllers
{
    public class MedicalHistoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
