using System.ComponentModel.DataAnnotations;

namespace KitDistributionAPI.DTOs
{
    public class BeneficiaryCreateDto
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        public string Mobile { get; set; }

        [Required]
        public string State_City { get; set; }
    }
}
