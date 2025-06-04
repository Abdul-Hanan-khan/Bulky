
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
        
            IEnumerable<Product> productList = _unitOfWork.ProductRepo.GetAll(includeProperties: "Category");
            return View(productList);
        }

        public IActionResult Details(int id)
        {

            int count = 0;
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            if (claimsIdentity.IsAuthenticated)
            {
                var userID = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                ShoppingCart existingProudctInCart = _unitOfWork.ShoppingCartRepo.Get(u => (u.ProductId == id && u.ApplicationUserId == userID));
                if (existingProudctInCart != null)
                {
                    count = existingProudctInCart.Count;
                }
            }



            ShoppingCart cart = new ShoppingCart
            {
                Product = _unitOfWork.ProductRepo.Get(u => u.Id == id, includeProperties: "Category"),
                Count = count,
                ProductId = id
            };
            return View(cart);
            //return View(product);

        }
        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart cart)
        {
            cart.Id = 0;
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userID = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            cart.ApplicationUserId = userID;

            ShoppingCart existingProudctInCart = _unitOfWork.ShoppingCartRepo.Get(u => (u.ProductId == cart.ProductId && u.ApplicationUserId == cart.ApplicationUserId));

            if (existingProudctInCart != null)
            {
                existingProudctInCart.Count = cart.Count;
                _unitOfWork.ShoppingCartRepo.Update(existingProudctInCart);
                _unitOfWork.Save();


            }
            else
            {
                _unitOfWork.ShoppingCartRepo.Add(cart);
                _unitOfWork.Save();

                HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCartRepo.GetAll(u => u.ApplicationUserId == userID).Count());
            }

            TempData["success"] = "Cart Update Successfully";

            return RedirectToAction(nameof(Index));
            // return View(cart);
            //return View(product);

        }
        [Authorize]
        public IActionResult DeleteProductFromCart(int productId)
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userID = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;


            ShoppingCart existingProudctInCart = _unitOfWork.ShoppingCartRepo.Get(u => (u.ProductId == productId && u.ApplicationUserId == userID));


            _unitOfWork.ShoppingCartRepo.Remove(existingProudctInCart);
            _unitOfWork.Save();
            //Product product = _unitOfWork.ProductRepo.Get(u => (u.Id == productId), includeProperties:"Category");

            //ShoppingCart cartModel = new ShoppingCart
            //{
            //    Product = product,
            //    ProductId= productId,
            //    Count=0,
            //};

            //return View("Details",cartModel);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View("Privacy");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
