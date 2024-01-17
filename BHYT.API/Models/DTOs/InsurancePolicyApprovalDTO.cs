namespace BHYT.API.Models.DTOs
{
    public class InsurancePolicyApprovalDTO
    {
        public int PolicyID { get; set; }

        public int CustomerID { get; set; }

        public string? CustomerName { get; set; }

        public string? CustomerBirthday { get; set; }

        public string? CustomerSex { get; set; }

        public string? CustomerPhone { get; set; }

        public string? CustomerEmail {  get; set; }

        public string? CustomerAdrress { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int? InsuranceID { get; set; }

        public string? InsuranceName { get; set; }

        public string? CreatedDate { get; set; }

        public string? PaymentOption { get; set; }

        public string? Status { get; set; }

    }
}

