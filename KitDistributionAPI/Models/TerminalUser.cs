using System.ComponentModel.DataAnnotations;

namespace KitDistributionAPI.Models
{
     [Table("terminalusers")] 
    public class TerminalUser
    {
        [Key]
        public string Terminal_ID { get; set; }

        public string FullName { get; set; }
        public string Password { get; set; }
        public string Status { get; set; }

        public DateTime Create_Date { get; set; }
        public DateTime? Update_Date { get; set; }
    }
}
