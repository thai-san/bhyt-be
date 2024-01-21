namespace BHYT.API.Models.DTOs
{
    public class ResetPasswordDTO
    {
        public string UserId { get; set; }
        public string ResetCode { get; set; }
        public string NewPassword { get; set; }
    }
}
