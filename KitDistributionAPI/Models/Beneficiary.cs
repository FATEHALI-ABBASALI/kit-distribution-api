using System.ComponentModel.DataAnnotations;

namespace KitDistributionAPI.Models
{
    [Table("beneficiaries")]
    public class Beneficiary
    {
        [Key]
        public string Beneficiary_ID { get; set; }

        public string Card_ID { get; set; }
        public string FullName { get; set; }
        public string Mobile { get; set; }
        public string State_City { get; set; }
        public string Password { get; set; }
        public string Status { get; set; }

        public DateTime Create_Date { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
