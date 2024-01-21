using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BHYT.API.Models.DbModels
{
    [Table("Account")]
    public class Account
    {
        public Account()
        {
            
        }
        [Key]
        public int Id { get; set; } 
        public required string Username { get; set; }

        public required string Password { get; set; }

        public int login_attempts { get; set; } //số lần đăng nhập sai

        public DateTime? last_failed_login { get; set; } //thời điểm đăng nhập sai gần nhất

        public bool is_locked { get; set; }// trạng thái khóa tài khoản(true/false)

        public DateTime? locked_until { get; set; }// thời điểm tài khoản được mở khóa
    }
}
