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
 
        //public required string Username { get; set; }

        //public required string Password { get; set; }

        //public int login_attempts { get; set; } //số lần đăng nhập sai
        //public DateTime? last_failed_login { get; set; } //thời điểm đăng nhập sai gần nhất
        //public bool is_locked { get; set; }// trạng thái khóa tài khoản(true/false)
        //public DateTime? locked_until { get; set; }// thời điểm tài khoản được mở khóa

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
