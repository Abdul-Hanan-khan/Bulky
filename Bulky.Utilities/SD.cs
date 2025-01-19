﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//SD stand for Static Detials
namespace Bulky.Utilities
{
	public static class SD
	{

		public const string localDomain = "https://localhost:7098/";
		
		
		public const string Role_Customer = "Customer";
		public const string Role_Company = "Company";
		public const string Role_Admin = "Admin";
		public const string Role_Employee = "Employee";

		public const string Status_Pending = "Pending";
		public const string Status_Approved = "Approved";
		public const string Status_Processing = "Processing";
		public const string Status_Shipped = "Shipped";
		public const string Status_Cancelled = "Cancelled";
		public const string Status_Refunded = "Refunded";

		public const string Payment_Status_Pending = "Pending";
		public const string Payment_Status_Approved = "Approved";
		public const string Payment_Status_DelayedPayment = "ApprovedForDelayedPayment";
		public const string Payment_Status_Rejected = "Rejected";




	}
}
