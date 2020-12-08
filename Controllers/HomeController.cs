using Microsoft.AspNetCore.Mvc;
using ZennoLab.Models;

namespace ZennoLab.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMetadataStorage _context;

        public HomeController(IMetadataStorage context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // one page only requirement 
            ViewData["FilesAdded"] = _context.RetrieveDataSetMetadata();
            ViewData["Message"] = TempData["Message"];
            return View();
        }
    }
}
