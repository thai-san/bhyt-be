using System.ComponentModel.DataAnnotations;

namespace BHYT.API.Models.DbModels
{
    public class User
    {
        public User()
        {
             
        }
        [Key]
        public int Id { get; set; }

        public Guid? Guid { get; set; }
 
        public string? Fullname { get; set; }

        public string? Address { get; set; }

        public string? Phone { get; set; }

        public DateTime? Birthday { get; set; }

        public int? Sex { get; set; }

        public string? Email { get; set; }

        public int? StatusId { get; set; }

        public int RoleId { get; set; }

        public int AccountId { get; set; }  

        public string? BankNumber { get; set; }

        public string? Bank { get; set; }

    }
}
