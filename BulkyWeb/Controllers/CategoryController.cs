using BulkyWeb.Data;
using BulkyWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext db)
        {
          _db = db;
        }
        public IActionResult Index()
        {
            List<Category> objCategoriesList = _db.Categories.ToList();
            //if (objCategoriesList.Count >0)
            //{
            //Console.WriteLine($"{objCategoriesList.Count} Categories Fetched");

            //}
            //else
            //{
            //Console.WriteLine($"No Record Found Against Categories");
            //}
            return View(objCategoriesList);
        }
    }
}
