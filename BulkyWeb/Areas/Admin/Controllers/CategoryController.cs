
using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Category> objCategoriesList = _unitOfWork.CategoryRepo.GetAll().ToList();

            return View(objCategoriesList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category obj)
        {
            if (!obj.Name.IsNullOrEmpty() && !obj.DisplayOrder.ToString().IsNullOrEmpty() && obj.Name.ToLower() == obj.DisplayOrder.ToString().ToLower())
            {
                ModelState.AddModelError("name", "Category Name and Display Order Can't Have Same Value");
            }

            if (ModelState.IsValid)
            {
                _unitOfWork.CategoryRepo.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = $"Category {obj.Name} Created Successfully";
                return RedirectToAction("Index");

            }
            else
            {
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

            //Category? categoryFromDb = _db.Categories.FirstOrDefault(u=>u.Id == id);
            Category? categoryFromDb = _unitOfWork.CategoryRepo.Get(u => u.Id == id);
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

        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.CategoryRepo.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = $"Category {obj.Name} Updated Successfully";

                return RedirectToAction("Index");
            }
            return View();
        }




        public IActionResult Delete(int? id)
        {
            //Category category = _db.Categories.Find(id);
            //Category? categoryFromDb1 = _db.Categories.Where(u => u.Id == id).FirstOrDefault();

            Category? categoryFromDb = _unitOfWork.CategoryRepo.Get(u => u.Id == id);
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
            Category? obj = _unitOfWork.CategoryRepo.Get(u => u.Id == id);

            if (obj == null)
            {
                return NotFound();
            }
            _unitOfWork.CategoryRepo.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = $"Category {obj.Name} Deleted Successfully";

            return RedirectToAction("Index");

        }


        public IActionResult Search(string searchQuery)
        {
            List<Category> filteredCategories;

            filteredCategories = _unitOfWork.CategoryRepo.searchCategory(searchQuery);

            //if (!searchQuery.IsNullOrEmpty())
            //{
            //    filteredCategories = _db.Categories
            //        .Where(c => c.Name.Contains(searchQuery))
            //        .ToList();
            //}
            //else
            //{
            //    filteredCategories = _db.Categories.ToList();
            //}



            return PartialView("_CategoryTable", filteredCategories); // Return partial view for the table
        }

    }
}
