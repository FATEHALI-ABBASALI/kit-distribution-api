using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KitDistributionAPI.Models
{
    [Table("admins")]   // ⭐ IMPORTANT
    public class Admin
    {
        [Key]
        public int Admin_ID { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
    }
}
