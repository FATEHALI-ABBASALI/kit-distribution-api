using System.ComponentModel.DataAnnotations;

namespace KitDistributionAPI.DTOs
{
    public class ResetPasswordDto
    {
        [Required]
        public string UserId { get; set; }
    }
}
