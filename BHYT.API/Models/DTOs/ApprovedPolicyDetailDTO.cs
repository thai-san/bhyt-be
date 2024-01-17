namespace BHYT.API.Models.DTOs
{
    public class ApprovedPolicyDetailDTO
    {
        public Guid guid { get; set; }
        public int policyId { get; set; }

        public int? customerId { get; set; }

        public string? customerName{ get; set; }
        public int? insuranceId { get; set; }

        public string? insuranceName { get; set; }
        public string? paymentOpption{ get; set; }
        public string? approvalDate { get; set; }
        public string? employeeName { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        


    }
}
