namespace BHYT.API.Models.DTOs
{
    public class UpdateUserStatusDTO
    {
        public int customerId { get; set; }
        public int newStatus { get; set; }
    }
}
