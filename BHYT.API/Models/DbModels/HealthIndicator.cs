using System.ComponentModel.DataAnnotations;

namespace BHYT.API.Models.DbModels
{
    public class HealthIndicator
    {
        public HealthIndicator() { }

        [Key]
        public int Id { get; set; }

        public Guid? Guid { get; set; }

        public int CustomerId { get; set; }

        public float? Height { get; set; } 

        public float? Weight { get; set; }

        public float? Cholesterol { get; set; }

        public float? BMI { get; set; }

        public int? BPM { get; set; } // nhịp tim

        public int? RespiratoryRate { get; set; }

        public string? Diseases {  get; set; }

        public DateTime? LastestUpdate { get; set; }
    }
}
