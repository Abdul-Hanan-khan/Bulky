
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
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
            _unitOfWork=unitOfWork; 
        }

        public IActionResult Index()
        {
            IEnumerable<Product> productList = _unitOfWork.ProductRepo.GetAll(includeProperties: "Category");
            return View(productList);
        }
        
        public IActionResult Details(int id)
        { 


            ShoppingCart cart = new ShoppingCart
            {
                Product = _unitOfWork.ProductRepo.Get(u => u.Id == id, includeProperties: "Category"),
                Count = 1,
                ProductId = id
            };
            return View(cart);
            //return View(product);

        }
        [HttpPost]
        [Authorize] // it ensure that user must login. no care about role but ensures login
        public IActionResult Details(ShoppingCart cart )
        {
            cart.Id =0;

            // get userID of the logged int user
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userID = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            cart.ApplicationUserId = userID;

            ShoppingCart existingProudctInCart = _unitOfWork.ShoppingCartRepo.Get(u=>(u.ProductId == cart.ProductId && u.ApplicationUserId == cart.ApplicationUserId));
            if (existingProudctInCart != null)
            {
                existingProudctInCart.Count += cart.Count;
                _unitOfWork.ShoppingCartRepo.Update(existingProudctInCart);
            }
            else {
                _unitOfWork.ShoppingCartRepo.Add(cart);
            }

            
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index) );
           // return View(cart);
            //return View(product);

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
