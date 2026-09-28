
using System.ComponentModel.DataAnnotations;


namespace TaskManagement.Models
{
    public class TwoFactorTab
    {
        public int Id { get; set; }

        public string UseType { get; set; }
        public int? MaxNumber { get; set; }
        public int? Seconds { get; set; }
        [MaxLength(100)]
        public string Code { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Description { get; set; } = string.Empty;
    }
}
