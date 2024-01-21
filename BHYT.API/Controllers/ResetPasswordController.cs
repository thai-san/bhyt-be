using BHYT.API.Models.DbModels;
using BHYT.API.Models.DTOs;
using BHYT.API.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BHYT.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResetPasswordController : ControllerBase
    {
        private readonly BHYTDbContext _context;
        private readonly IEmailService _emailService;

        public ResetPasswordController(BHYTDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [HttpPost("request-reset")]
        public async Task<IActionResult> RequestPasswordReset(ResetPasswordRequestDTO dto)
        {
            try
            {
                var user = await _context.Users
                       .Join(
                           _context.Accounts,
                           user => user.AccountId,
                           account => account.Id,
                           (user, account) => new { User = user, Account = account }
                       )
                       .Where(a => a.User.Email == dto.Email)
                       .FirstOrDefaultAsync();

                if (user == null)
                {
                    return NotFound(new ApiResponse { Message = "Invalid email address." });
                }

                // Tạo yêu cầu đặt lại mật khẩu
                var resetRequest = new ResetPasswordRequest
                {
                    UserId = user.User.Id.ToString(),
                    Guid = Guid.NewGuid(),
                    Resetrequestcode = GenerateResetCode(),
                    Requestdate = DateTime.UtcNow
                };

                _context.ResetPasswordRequests.Add(resetRequest);
                await _context.SaveChangesAsync();

                // Gửi mã đặt lại cho người dùng (có thể gửi qua email hoặc thông báo khác)
                EmailDTO emailDTO = new EmailDTO
                {
                    ToEmail = user.User.Email,
                    Subject = "KHÔI PHỤC MẬT KHẨU",
                    Body = $"<h1>Xin chào {user.Account.Username}, Mã khôi phục tài khoản của bạn là: {resetRequest.Resetrequestcode}</h1>"
                };
                await _emailService.SendEmailAsync(emailDTO);

                return Ok(new ApiResponse { Success = true, Message = "Password reset request sent to email successfully.", Data = new Object[] { user.Account.Id, resetRequest.Resetrequestcode } });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO dto)
        {
            try
            {
                // Kiểm tra sự tồn tại của yêu cầu đặt lại dựa trên userId và reset code
                var resetRequest = await _context.ResetPasswordRequests
                    .Where(r => r.UserId == dto.UserId && r.Resetrequestcode == dto.ResetCode)
                    .OrderByDescending(r => r.Requestdate)
                    .FirstOrDefaultAsync();

                if (resetRequest == null || resetRequest.Requestdate == null || resetRequest.Requestdate.Value.AddHours(24) < DateTime.UtcNow)
                {
                    return BadRequest(new ApiResponse { Message = "Invalid or expired reset code." });
                }

                // Cập nhật mật khẩu cho tài khoản
                var user = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == Convert.ToUInt32(resetRequest.UserId));

                if (user != null)
                {
                    // Kiểm tra tính hợp lệ của mật khẩu mới
                    if (IsValidPassword(dto.NewPassword))
                    {
                        user.Password = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
                        await _context.SaveChangesAsync();

                        // Đánh dấu yêu cầu đặt lại đã được sử dụng
                        resetRequest.Resetdate = DateTime.UtcNow;
                        await _context.SaveChangesAsync();

                        return Ok(new ApiResponse { Success = true, Message = "Password reset successfully." });
                    }
                    else
                    {
                        return BadRequest(new ApiResponse { Message = "Invalid new password format." });
                    }
                }

                return NotFound(new ApiResponse { Message = "User not found." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        private bool IsValidPassword(string password)
        {
            // Kiểm tra tính hợp lệ của mật khẩu, ví dụ: ít nhất 8 ký tự, có ít nhất một chữ cái và một số
            return !string.IsNullOrEmpty(password) && password.Length >= 6;
        }

        // Hàm tạo mã đặt lại, bạn có thể thay đổi theo nhu cầu của mình
        private string GenerateResetCode()
        {
            // Logic tạo mã
            return Guid.NewGuid().ToString("N").Substring(0, 8);
        }
    }
}
