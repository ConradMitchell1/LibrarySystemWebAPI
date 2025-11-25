using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystemWebAPI.Controllers
{
    public class PageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "User,Admin")]
        public IActionResult UserAccount()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Dashboard()
        {
            return View();
        }
        [Authorize(Roles = "User,Admin")]
        public IActionResult Library()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        public IActionResult BorrowList(int id)
        {
            ViewBag.BookId = id;
            return View();
        }
        [Authorize(Roles = "Admin")]
        public IActionResult TopBorrowed()
        {
            return View();
        }
    }
}
