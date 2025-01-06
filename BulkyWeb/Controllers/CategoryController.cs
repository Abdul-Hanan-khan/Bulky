using BulkyWeb.Data;
using BulkyWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

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
         
            return View(objCategoriesList);
        }

        public IActionResult Create() {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category obj) {
            if (!obj.Name.IsNullOrEmpty() && !obj.DisplayOrder.ToString().IsNullOrEmpty() && obj.Name.ToLower() == obj.DisplayOrder.ToString().ToLower()) {
                ModelState.AddModelError("name","Category Name and Display Order Can't Have Same Value");
            }

            if (ModelState.IsValid)
            {
                _db.Categories.Add(obj);
                _db.SaveChanges();
                TempData["success"] = $"Category {obj.Name} Created Successfully";
                return RedirectToAction("Index");

            }
            else {
                Console.WriteLine("Server side validation is failed. Model is not Validated");
                return View();
            }

            // do this if your controller is different
            //  return RedirectToAction("Index","Category");  
        }


		public IActionResult Edit(int? id)
		{
			//Category category = _db.Categories.Find(id);
			//Category? categoryFromDb1 = _db.Categories.Where(u => u.Id == id).FirstOrDefault();

			Category? categoryFromDb = _db.Categories.FirstOrDefault(u=>u.Id == id);
			if (id == null)
            {
                return NotFound();
			}

			if (categoryFromDb == null) {
                return NotFound();
            }

			return View(categoryFromDb);
		}

        [HttpPost]
        public IActionResult Edit(Category obj) {
            if (ModelState.IsValid) {
                _db.Categories.Update(obj);
                _db.SaveChanges();
				TempData["success"] = $"Category {obj.Name} Updated Successfully";

				return RedirectToAction("Index");
            }
            return View();
        }




		public IActionResult Delete(int? id)
		{
			//Category category = _db.Categories.Find(id);
			//Category? categoryFromDb1 = _db.Categories.Where(u => u.Id == id).FirstOrDefault();

			Category? categoryFromDb = _db.Categories.FirstOrDefault(u => u.Id == id);
			if (id == null)
			{
				return NotFound();
			}

			if (categoryFromDb == null)
			{
				return NotFound();
			}

			return View(categoryFromDb);
		}

		[HttpPost, ActionName("Delete")]
		public IActionResult DeletePost(int? id)
		{
            Category ?obj = _db.Categories.Find(id);

            if (obj == null) {
                return NotFound();
            }
				_db.Categories.Remove(obj);
				_db.SaveChanges();
			TempData["success"] = $"Category {obj.Name} Deleted Successfully";

			return RedirectToAction("Index");
			
		}


		public IActionResult Search(string searchQuery)
		{
            List<Category> filteredCategories;

            if (!searchQuery.IsNullOrEmpty())
            {
                filteredCategories = _db.Categories
                    .Where(c => c.Name.Contains(searchQuery))
                    .ToList();
            }
            else { 
            filteredCategories = _db.Categories.ToList();
            }

			

			return PartialView("_CategoryTable", filteredCategories); // Return partial view for the table
		}

	}
}
