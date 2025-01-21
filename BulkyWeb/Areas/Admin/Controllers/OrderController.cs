using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using NuGet.Versioning;
using Stripe;
using Stripe.Checkout;
using Stripe.Climate;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize]
	public class OrderController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		[BindProperty]
		public OrderVM orderVM { get; set; }
		public OrderController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}
		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Details(int orderId)
		{
			OrderVM orderVM = new OrderVM()
			{
				OrderHeader = _unitOfWork.OrderHeaderRepo.Get(u => u.Id == orderId, includeProperties: "ApplicationUser"),
				OrderDetails = _unitOfWork.OrderDetailRepo.GetAll(u => u.OrderHeaderId == orderId, includeProperties: "Product")
			};


			;
			return View(orderVM);
		}
		[HttpPost]
		[Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]

		public IActionResult UpdateOrderDetails()
		{
			var orderHeaderFromDb = _unitOfWork.OrderHeaderRepo.Get(u => u.Id == orderVM.OrderHeader.Id);
			orderHeaderFromDb.Name = orderVM.OrderHeader.Name;
			orderHeaderFromDb.PhoneNumber = orderVM.OrderHeader.PhoneNumber;
			orderHeaderFromDb.StreetAddress = orderVM.OrderHeader.StreetAddress;
			orderHeaderFromDb.City = orderVM.OrderHeader.City;
			orderHeaderFromDb.State = orderVM.OrderHeader.State;
			orderHeaderFromDb.PostalCode = orderVM.OrderHeader.PostalCode;

			if (!string.IsNullOrEmpty(orderVM.OrderHeader.CarrierInformation))
			{
				orderHeaderFromDb.CarrierInformation = orderVM.OrderHeader.CarrierInformation;
			}
			if (!string.IsNullOrEmpty(orderVM.OrderHeader.TrackingNumber))
			{
				orderHeaderFromDb.TrackingNumber = orderVM.OrderHeader.TrackingNumber;
			}

			_unitOfWork.OrderHeaderRepo.Update(orderHeaderFromDb);
			_unitOfWork.Save();
			TempData["success"] = "Order Details Updated Successfully";
			return RedirectToAction(nameof(Details), new { orderId = orderHeaderFromDb.Id });
		}

		[HttpPost]
		[Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
		public IActionResult StartProcessing()
		{
			_unitOfWork.OrderHeaderRepo.UpdateStatus(orderVM.OrderHeader.Id, SD.Status_Processing);
			_unitOfWork.Save();
			TempData["success"] = "Order Details Updated Successfully";
			return RedirectToAction(nameof(Details), new { orderId = orderVM.OrderHeader.Id });
		}




		[HttpPost]
		[Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
		public IActionResult ShipOrder()
		{

			var orderHeader = _unitOfWork.OrderHeaderRepo.Get(u => u.Id == orderVM.OrderHeader.Id);

			orderHeader.TrackingNumber = orderVM.OrderHeader.TrackingNumber;
			orderHeader.CarrierInformation = orderVM.OrderHeader.CarrierInformation;
			orderHeader.OrderStatus = SD.Status_Shipped;

			orderHeader.ShippingDate = DateTime.Now;

			if (orderHeader.PaymentStatus == SD.Payment_Status_DelayedPayment)
			{
				orderHeader.PaymentDueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(30));
			}

			_unitOfWork.OrderHeaderRepo.Update(orderHeader);
			_unitOfWork.Save();
			TempData["success"] = "Order Shipped Successfully";
			return RedirectToAction(nameof(Details), new { orderId = orderVM.OrderHeader.Id });
		}

		[HttpPost]
		[Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
		public IActionResult CancelOrder()
		{
			var orderHeader = _unitOfWork.OrderHeaderRepo.Get(u => u.Id == orderVM.OrderHeader.Id);
			if (orderHeader.PaymentStatus == SD.Payment_Status_Approved)
			{
				// payment is already done. start refund process also
				var options = new RefundCreateOptions()
				{
					Reason = RefundReasons.RequestedByCustomer,
					PaymentIntent = orderHeader.PaymentIntentId,

				};

				var service = new RefundService();
				Refund refund = service.Create(options);
				_unitOfWork.OrderHeaderRepo.UpdateStatus(orderHeader.Id, SD.Status_Cancelled, SD.Status_Refunded);
			}
			else
			{
				_unitOfWork.OrderHeaderRepo.UpdateStatus(orderHeader.Id, SD.Status_Cancelled);

			}

			_unitOfWork.Save();
			TempData["success"] = "Order Cancelled Successfully";
			return RedirectToAction(nameof(Details), new { orderId = orderVM.OrderHeader.Id });
		}

		[HttpPost]
		[ActionName("Details")]
		public IActionResult Details_Pay_Now()
		{

			OrderVM orderVMFromDb = new OrderVM
			{
				OrderHeader = _unitOfWork.OrderHeaderRepo.Get(u => u.Id == orderVM.OrderHeader.Id, includeProperties: "ApplicationUser"),
				OrderDetails = _unitOfWork.OrderDetailRepo.GetAll(u => u.OrderHeaderId == orderVM.OrderHeader.Id, includeProperties: "Product")
			};

			var options = new Stripe.Checkout.SessionCreateOptions
			{
				SuccessUrl = SD.localDomain + $"admin/order/PaymentConfirmation?orderHeaderId={orderVMFromDb.OrderHeader.Id}",
				CancelUrl = SD.localDomain + $"admin/order/details?orderId={orderVMFromDb.OrderHeader.Id}",

				LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
				Mode = "payment",
			};

			foreach (var item in orderVMFromDb.OrderDetails)
			{
				var sessionLineItem = new SessionLineItemOptions
				{
					PriceData = new SessionLineItemPriceDataOptions
					{
						UnitAmount = (long)(item.Price * 100), // 12.50 => 1250
						Currency = "usd",
						ProductData = new SessionLineItemPriceDataProductDataOptions
						{
							Name = item.Product.Title,

						}
					},
					Quantity = item.Count,
				};
				options.LineItems.Add(sessionLineItem);
			}

			var service = new Stripe.Checkout.SessionService();
			Session session = service.Create(options);


			// session id is generated when every there is an attempt to make payment.
			// payment intent is generated only if payment is successfull.

			_unitOfWork.OrderHeaderRepo.UpdateStripePaymentId(orderVMFromDb.OrderHeader.Id, session.Id, session.PaymentIntentId);
			_unitOfWork.Save();

			Response.Headers.Add("Location", session.Url);
			return new StatusCodeResult(303);

			//return RedirectToAction(nameof(Details), new { orderId = orderVM.OrderHeader.Id });

		}


		public IActionResult PaymentConfirmation(int orderHeaderId)
		{
			OrderHeader orderHeader = _unitOfWork.OrderHeaderRepo.Get(u => u.Id == orderHeaderId, includeProperties: "ApplicationUser");
			if (orderHeader.PaymentStatus == SD.Payment_Status_DelayedPayment)
			{
				// Company Payemnt
				var service = new SessionService();
				Session session = service.Get(orderHeader.SessionId);
				if (session.PaymentStatus.ToLower() == "paid")
				{
					_unitOfWork.OrderHeaderRepo.UpdateStripePaymentId(orderHeaderId, session.Id, session.PaymentIntentId);
					_unitOfWork.OrderHeaderRepo.UpdateStatus(orderHeaderId, orderHeader.OrderStatus, SD.Payment_Status_Approved);
					_unitOfWork.Save();

				}
			}
			return View(orderHeaderId);
		}

		#region API CALLS
		[HttpGet]
		public IActionResult GetAll(string status)
		{
			IEnumerable<OrderHeader> objOrderHeaders;

			if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
			{
				objOrderHeaders = _unitOfWork.OrderHeaderRepo.GetAll(includeProperties: "ApplicationUser").ToList();
			}
			else
			{
				var claimsIdentity = (ClaimsIdentity)User.Identity;
				var userID = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
				objOrderHeaders = _unitOfWork.OrderHeaderRepo.GetAll(u => u.ApplicationUserId == userID, includeProperties: "ApplicationUser").ToList();
			}
			switch (status)
			{
				case "inprocess":
					objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.Status_Processing);
					break;
				case "pending":
					objOrderHeaders = objOrderHeaders.Where(u => u.PaymentStatus == SD.Payment_Status_DelayedPayment);
					break;
				case "approved":
					objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.Status_Approved);
					break;
				case "completed":
					objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.Status_Shipped);
					break;
				case "all":
					break;
				default:
					break;
			}

			return Json(new { data = objOrderHeaders });
		}



		#endregion

	}
}
