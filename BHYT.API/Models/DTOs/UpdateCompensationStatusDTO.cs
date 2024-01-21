namespace BHYT.API.Models.DTOs
{
    public class UpdateCompensationStatusDTO
    {
        public int compensationId { get; set; }
        public bool? newStatus { get; set; }
    }
}
