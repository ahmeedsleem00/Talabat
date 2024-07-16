﻿using System.ComponentModel.DataAnnotations;
using Talabat.Core.Entities;

namespace Talabat.APIs.Dtos
{
    public class CustomerBasketDto
    {
        [Required]
        public string Id { get; set; }

        public List<BasketItem> Items { get; set; }

        public int? DeliveryMethodId { get; set; }

        public string? PaymentIntentId { get; set; }
        public string? ClientSecret { get; set; }
    }
}
