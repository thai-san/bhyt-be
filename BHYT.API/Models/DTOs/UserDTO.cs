namespace BHYT.API.Models.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }

        public Guid? Guid { get; set; }

        public required string Username { get; set; }

        public required string Password { get; set; }

        public string? Fullname { get; set; }

        public string? Address { get; set; }

        public string? Phone { get; set; }

        public DateTime? Birthday { get; set; }

        public int? Sex { get; set; }

        public string? Email { get; set; }

        public int? StatusId { get; set; }
    }
}
