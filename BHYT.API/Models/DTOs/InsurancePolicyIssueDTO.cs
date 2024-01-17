namespace BHYT.API.Models.DTOs
{
    public class InsurancePolicyIssueDTO
    {

        public int policyId {  get; set; }
        public bool? PaymentOption { get; set; }  // loại thanh toán true: Tháng, false : năm

        public int? InsuranceId { get; set; }  

        public string? Description { get; set; } //null

        public bool? Status { get; set; }

        public string? sex { get; set; }

        public string? birthday { get; set; }

    }
}
