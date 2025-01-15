
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
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            //List<Product> objProductList = _unitOfWork.ProductRepo.GetAll(includeProperties:"Category").ToList();

            //return View(objProductList);
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            IEnumerable<SelectListItem> categoryList = _unitOfWork.CategoryRepo.GetAll().Select(item => new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
            //ViewBag.CategoryList = categoryList;
            //ViewData["CategoryList"] = categoryList;

            ProductVM productVM = new ProductVM { CategoryList = categoryList, Product = new Product() };

            if (id == null || id == 0)
            {
                // insert/create operation
                return View(productVM);
            }
            else
            {
                //update operation;
                productVM.Product = _unitOfWork.ProductRepo.Get(item => item.Id == id);
                return View(productVM);

            }

        }

        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, IFormFile? file)
        {


            if (ModelState.IsValid)
            {
                string wwwrootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwrootPath, @"Images\Product");
                    if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                        // it means image is already there and new image should be populated
                        var oldImagePath = Path.Combine(wwwrootPath, productVM.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    productVM.Product.ImageUrl = @"\Images\Product\" + fileName;

                }

                if (productVM.Product.Id == 0)
                {

                    productVM.Product.ImageUrl ??= "";
                    _unitOfWork.ProductRepo.Add(productVM.Product);
                }
                else
                {
                    _unitOfWork.ProductRepo.Update(productVM.Product);
                }
                _unitOfWork.Save();
                TempData["success"] = "Completed Successfully";
                return RedirectToAction("Index");

            }
            else
            {
                Console.WriteLine($"Server side validation is failed. Model is not Validated {ModelState.ErrorCount}");
                productVM.CategoryList = _unitOfWork.CategoryRepo.GetAll().Select(item => new SelectListItem { Text = item.Name, Value = item.Id.ToString() });

                return View(productVM);
            }

            // do this if your controller is different
            //  return RedirectToAction("Index","Category");  
        }



        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> objProductList = _unitOfWork.ProductRepo.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = objProductList });
        }


        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productToBeDelete = _unitOfWork.ProductRepo.Get(u => u.Id == id);

            if (productToBeDelete == null)
            {
                return Json(new { sucess = false, message = "Error Occured While Deleting The Product" });
            }

            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, productToBeDelete.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }

            _unitOfWork.ProductRepo.Remove(productToBeDelete);
            _unitOfWork.Save();
            
            return Json(new { success = true, message = "Product Deleted Successfully"});
        }
        #endregion

    }
}
