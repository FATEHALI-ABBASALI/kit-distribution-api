using System.ComponentModel.DataAnnotations;

namespace KitDistributionAPI.Models
{
    public class Admin
    {
        [Key]
        public int Admin_ID { get; set; }

        public string FullName { get; set; }
        public string Password { get; set; }
    }
}
