﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models
{
	public class ShoppingCart
	{
		public int Id { get; set; }

		public int ProductId { get; set; }
		[ForeignKey("ProductId")]
		[ValidateNever]
        public Product Product { get; set; }
		[Range(1,1000, ErrorMessage ="Please Enter a Value Between 1 and 1000")]
        public int Count { get; set; }

		public string ApplicationUserId { get; set; }
		[ForeignKey("ApplicationUserId")]
		[ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }

        [NotMapped]
        public double  Price { get; set; }

        //public bool IsCartClosed { get; set; }


    }
}
