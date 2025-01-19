using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShoppingCartVM  ShoppingCartVM { get; set; }

        public CartController(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;   
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userID = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            ShoppingCartVM = new ShoppingCartVM
            {
                ShoppingCartList = _unitOfWork.ShoppingCartRepo.GetAll(u=> u.ApplicationUserId == userID,includeProperties:"Product"),
            };

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = getPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderTotal += (cart.Price * cart.Count);
            }

            return View(ShoppingCartVM);
        }


        public IActionResult plus(int cartId) {
            var cartFromDb = _unitOfWork.ShoppingCartRepo.Get(u=>u.Id == cartId);
            cartFromDb.Count += 1;
            _unitOfWork.ShoppingCartRepo.Update(cartFromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult minus(int cartId) {
            var cartFromDb = _unitOfWork.ShoppingCartRepo.Get(u=>u.Id == cartId);

            if (cartFromDb.Count == 1)
            {
                _unitOfWork.ShoppingCartRepo.Remove(cartFromDb);
            }
            else {
                cartFromDb.Count -= 1;
                _unitOfWork.ShoppingCartRepo.Update(cartFromDb);
                
            }

            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }    
        public IActionResult remove(int cartId) {
            var cartFromDb = _unitOfWork.ShoppingCartRepo.Get(u=>u.Id == cartId);
            
            _unitOfWork.ShoppingCartRepo.Remove(cartFromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }


        private double getPriceBasedOnQuantity(ShoppingCart cartItem) {
            if (cartItem.Count <= 50)
            {
                return cartItem.Product.Price;
            }
            else {
                if (cartItem.Count <= 100)
                {
                    return cartItem.Product.Price50;
                }
                else {
                    return cartItem.Product.Price100;
                }
            }
        }
    }
}
