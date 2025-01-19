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
				ShoppingCartList = _unitOfWork.ShoppingCartRepo.GetAll(u => u.ApplicationUserId == userID, includeProperties: "Product"),
				OrderHeader = new OrderHeader()
			};

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = getPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            return View(ShoppingCartVM);
        }


		public IActionResult Summary ()  {

			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userID = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
			ShoppingCartVM = new ShoppingCartVM
			{
				ShoppingCartList = _unitOfWork.ShoppingCartRepo.GetAll(u => u.ApplicationUserId == userID, includeProperties: "Product"),
				OrderHeader = new OrderHeader()
			};

			ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUserRepo.Get(u=>u.Id == userID,includeProperties:"Company");

			ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
			ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
			ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.Address;
			ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
			ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
			ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;


			foreach (var cart in ShoppingCartVM.ShoppingCartList)
			{
				cart.Price = getPriceBasedOnQuantity(cart);
				ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
			}



			return View(ShoppingCartVM);
		}


		[HttpPost]
		public IActionResult Plus(int cartId)
		{
			var cartFromDb = _unitOfWork.ShoppingCartRepo.Get(u => u.Id == cartId);
			if (cartFromDb == null)
			{
				return Json(new { success = false, message = "Item not found" });
			}

			cartFromDb.Count += 1;
			_unitOfWork.ShoppingCartRepo.Update(cartFromDb);
			_unitOfWork.Save();

			var updatedCartTotal = _unitOfWork.ShoppingCartRepo.GetAll(
				u => u.ApplicationUserId == cartFromDb.ApplicationUserId,
				includeProperties: "Product"
			).Sum(c => c.Count * getPriceBasedOnQuantity(c));

			return Json(new
			{
				success = true,
				updatedCount = cartFromDb.Count,
				updatedTotal = updatedCartTotal
			});
		}

		[HttpPost]
		public IActionResult Minus(int cartId)
		{
			var cartFromDb = _unitOfWork.ShoppingCartRepo.Get(u => u.Id == cartId);
			if (cartFromDb == null)
			{
				return Json(new { success = false, message = "Item not found" });
			}

			if (cartFromDb.Count == 1)
			{
				_unitOfWork.ShoppingCartRepo.Remove(cartFromDb);
			}
			else
			{
				cartFromDb.Count -= 1;
				_unitOfWork.ShoppingCartRepo.Update(cartFromDb);
			}
			_unitOfWork.Save();

			var updatedCartTotal = _unitOfWork.ShoppingCartRepo.GetAll(
				u => u.ApplicationUserId == cartFromDb.ApplicationUserId,
				includeProperties: "Product"
			).Sum(c => c.Count * getPriceBasedOnQuantity(c));

			return Json(new
			{
				success = true,
				updatedCount = cartFromDb.Count,
				updatedTotal = updatedCartTotal
			});
		}

		[HttpPost]
		public IActionResult Remove(int cartId)
		{
			var cartFromDb = _unitOfWork.ShoppingCartRepo.Get(u => u.Id == cartId);
			if (cartFromDb == null)
			{
				return Json(new { success = false, message = "Item not found" });
			}

			_unitOfWork.ShoppingCartRepo.Remove(cartFromDb);
			_unitOfWork.Save();

			var updatedCartTotal = _unitOfWork.ShoppingCartRepo.GetAll(
				u => u.ApplicationUserId == cartFromDb.ApplicationUserId,
				includeProperties: "Product"
			).Sum(c => c.Count * getPriceBasedOnQuantity(c));

			return Json(new
			{
				success = true,
				updatedTotal = updatedCartTotal
			});
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
