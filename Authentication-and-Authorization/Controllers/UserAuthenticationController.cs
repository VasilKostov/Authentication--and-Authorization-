using Microsoft.AspNetCore.Mvc;

namespace Authentication_and_Authorization.Controllers
{
    public class UserAuthenticationController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
    }
}
