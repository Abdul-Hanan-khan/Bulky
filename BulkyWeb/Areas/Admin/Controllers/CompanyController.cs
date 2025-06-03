
using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin) ]
    
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Company> objCompanyList = _unitOfWork.CompanyRepo.GetAll().ToList();

            return View(objCompanyList);

        }

        public IActionResult Upsert(int? id)
        {
            IEnumerable<SelectListItem> categoryList = _unitOfWork.CategoryRepo.GetAll().Select(item => new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
            //ViewBag.CategoryList = categoryList;
            //ViewData["CategoryList"] = categoryList;

           

            if (id == null || id == 0)
            {
                // insert/create operation
                return View(new Company());
            }
            else
            {
                //update operation;
                 Company companyObj = _unitOfWork.CompanyRepo.Get(item => item.Id == id);
                return View(companyObj);

            }

        }

        [HttpPost]
        public IActionResult Upsert(Company companyObj)
        {


            if (ModelState.IsValid)
            {
                
                

                if (companyObj.Id == 0)
                {

                    
                    _unitOfWork.CompanyRepo.Add(companyObj);
                }
                else
                {
                    _unitOfWork.CompanyRepo.Update(companyObj);
                }
                _unitOfWork.Save();
                TempData["success"] = "Completed Successfully";
                return RedirectToAction("Index");

            }
            else
            {
                Console.WriteLine($"Server side validation is failed. Model is not Validated {ModelState.ErrorCount}");
             
                return View(companyObj);
            }

            // do this if your controller is different
            //  return RedirectToAction("Index","Category");  
        }



        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Company> objCompanyList = _unitOfWork.CompanyRepo.GetAll().ToList();
            return Json(new { data = objCompanyList });
        }


        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var companyToBeDelete = _unitOfWork.CompanyRepo.Get(u => u.Id == id);

            if (companyToBeDelete == null)
            {
                return Json(new { sucess = false, message = "Error Occured While Deleting The Company" });
            }

           

            _unitOfWork.CompanyRepo.Remove(companyToBeDelete);
            _unitOfWork.Save();
            
            return Json(new { success = true, message = "Company Deleted Successfully"});
        }
        #endregion

    }
}
