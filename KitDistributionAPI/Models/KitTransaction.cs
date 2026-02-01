using System.ComponentModel.DataAnnotations;

namespace KitDistributionAPI.Models
{
    public class KitTransaction
    {
        [Key]
        public int Id { get; set; }

        public string Beneficiary_ID { get; set; }
        public string Terminal_ID { get; set; }
        public string Month { get; set; }
        public DateTime Date { get; set; }
        public int Amount { get; set; }
        public string Status { get; set; }
    }
}
