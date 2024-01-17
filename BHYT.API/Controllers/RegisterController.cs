using AutoMapper;
using Azure.Core;
using BHYT.API.Models.DbModels;
using BHYT.API.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Data;

namespace BHYT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly BHYTDbContext _context ;
        private readonly IMapper _mapper ;
        public RegisterController(BHYTDbContext context,  IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> register(RegisterDTO dto)
        {
            var checkAccount = await _context.Accounts.FirstOrDefaultAsync(u => u.Username == dto.Username);
            if (checkAccount != null)
                return BadRequest(
                    new ApiResponse { Message = "User already exists" }
                );

            var checkEmail = _context.Users.Any(user => user.Email == dto.Email);
            if (checkEmail)
                return BadRequest(new ApiResponse { Message = "Email already exists for other account" });
            try
            {
                // add new account
                Account account = new Account()
                {
                    Username = dto.Username,
                    Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                    login_attempts = 0,
                };
                await _context.Accounts.AddAsync(account);
                await _context.SaveChangesAsync();

                // add new user
                User newUser = new User()
                {
                    Email = dto.Email,
                    Guid = new Guid(),
                    StatusId = 1,
                    AccountId = account.Id,
                    RoleId = 2,  // default customer , empolyee can't sign up
                };

                await _context.Users.AddAsync(newUser);
                await _context.SaveChangesAsync();

                return Ok( new
                {
                    Message = "register successfully!"
                });
            }
            catch (Exception ex)
            {
                return Conflict(new 
                {
                    Message = "can't create new user!"
                });
                throw;
            }
        }
    }
}
