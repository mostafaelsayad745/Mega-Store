using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegaStore.Entities.Models
{
	public class OrderHeader
	{
		public int Id { get; set; }

        // User who made the order
        public string ApplicationUserId { get; set; }
		[ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }

        // Date when the user make the order .
        public DateTime OrderDate { get; set; }
        // Date when the order is shipped .
        public DateTime ShippingDate { get; set; }
        // total count of the order .
        public decimal TotalPrice { get; set; }
        // weather the order is enabled or disabled
        public string? OrderStatus { get; set; }
        // to check weather the payment of the order is done or not .
        public string? PaymentStatus { get; set; }
        // is used to track the order 
        public string? TrackingNumber { get; set; }
        // who is responsilbe fo shipping 
        public string? CarrierI{ get; set; }
        // the date when the payment is done .
        public DateTime PaymentDate { get; set; }

        // Stripe Properties

        public string? SessionId { get; set; }

        public string? PaymentIntentId { get; set;}

		// Data of User

		public string Name { get; set; }
		public string City { get; set; }
		public string Address { get; set; }
        public string Phone { get; set; }
	}
}
