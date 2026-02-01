using System;
using System.ComponentModel.DataAnnotations;

namespace KitDistributionAPI.DTOs
{
    public class DistributeKitDto
    {
        [Required]
        public string Beneficiary_ID { get; set; }

        // ❌ DO NOT send Terminal_ID from frontend
        // ✅ Terminal_ID is taken from JWT token in controller

        [Required]
        public DateTime Date { get; set; }
    }
}
