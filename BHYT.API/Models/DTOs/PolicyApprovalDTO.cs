namespace BHYT.API.Models.DTOs
{
    public class PolicyApprovalDTO
    {

        public Guid? Guid { get; set; }

        public int? PolicyId { get; set; }

        public int? EmployeeId { get; set; }

        public DateTime? ApprovalDate { get; set; }

        public int? StatusId { get; set; }
    }
}
