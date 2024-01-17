namespace BHYT.API.Models.DTOs
{
    public class HealthHistoryDTO
    {
        public Guid? Guid { get; set; }

        public int? CustomerId { get; set; }

        public int? InsuranceId { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string? Detail { get; set; }

        public string? Note { get; set; }

        public string? Diagnostic { get; set; }

        public string? HospitalNumber { get; set; }

        public string? Condition { get; set; }
    }
}
