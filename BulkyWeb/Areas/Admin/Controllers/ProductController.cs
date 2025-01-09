﻿
using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Product> objProductList = _unitOfWork.ProductRepo.GetAll().ToList();

            return View(objProductList);
        }

        public IActionResult Create()
        {
			IEnumerable<SelectListItem> categoryList = _unitOfWork.CategoryRepo.GetAll().Select(item => new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
            //ViewBag.CategoryList = categoryList;
            //ViewData["CategoryList"] = categoryList;

            ProductVM productVM = new ProductVM { CategoryList = categoryList, Product = new Product()};

			return View(productVM);
        }

        [HttpPost]
        public IActionResult Create(ProductVM obj)
        {
          

            if (ModelState.IsValid)
            {
                _unitOfWork.ProductRepo.Add(obj.Product);
                _unitOfWork.Save();
                TempData["success"] = $"Product {obj.Product.Title} Created Successfully";
                return RedirectToAction("Index");

            }
            else
            {
                Console.WriteLine($"Server side validation is failed. Model is not Validated {ModelState.ErrorCount}");
                obj.CategoryList = _unitOfWork.CategoryRepo.GetAll().Select(item => new SelectListItem { Text = item.Name, Value = item.Id.ToString() });

				return View(obj);
            }

            // do this if your controller is different
            //  return RedirectToAction("Index","Category");  
        }


        public IActionResult Edit(int? id)
        {
            //Category category = _db.Categories.Find(id);
            //Category? categoryFromDb1 = _db.Categories.Where(u => u.Id == id).FirstOrDefault();

            //Category? categoryFromDb = _db.Categories.FirstOrDefault(u=>u.Id == id);
            Product? productFromDb = _unitOfWork.ProductRepo.Get(u => u.Id == id);
            if (id == null)
            {
                return NotFound();
            }

            if (productFromDb == null)
            {
                return NotFound();
            }

            return View(productFromDb);
        }

        [HttpPost]
        public IActionResult Edit(Product obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.ProductRepo.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = $"Product {obj.Title} Updated Successfully";

                return RedirectToAction("Index");
            }
            return View();
        }




        public IActionResult Delete(int? id)
        {
            //Category category = _db.Categories.Find(id);
            //Category? categoryFromDb1 = _db.Categories.Where(u => u.Id == id).FirstOrDefault();

            Product? prouductFromDb = _unitOfWork.ProductRepo.Get(u => u.Id == id);
            if (id == null)
            {
                return NotFound();
            }

            if (prouductFromDb == null)
            {
                return NotFound();
            }

            return View(prouductFromDb);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            Product? obj = _unitOfWork.ProductRepo.Get(u => u.Id == id);

            if (obj == null)
            {
                return NotFound();
            }
            _unitOfWork.ProductRepo.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = $"Product {obj.Title} Deleted Successfully";

            return RedirectToAction("Index");

        }


        public IActionResult Search(string searchQuery)
        {
            List<Product> filteredProducts;

            filteredProducts = _unitOfWork.ProductRepo.SearchProduct(searchQuery);

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



            return PartialView("_ProductTable", filteredProducts); // Return partial view for the table
        }

    }
}
