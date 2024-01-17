using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BHYT.API.Models.DbModels
{
    public class RefreshToken
    {
        [Key]
        public Guid Id { get; set; }
        public int AccountId { get; set; } // acount id
        public string Token { get; set; }
        public string AccessTokenId { get; set; }
        public bool IsUsed { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime IssuedAt { get; set; }
        public DateTime ExpiredAt { get; set; }
    }
}
