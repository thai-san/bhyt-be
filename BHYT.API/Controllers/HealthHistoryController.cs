using AutoMapper;
using AutoMapper.QueryableExtensions;
using BHYT.API.Models.DbModels;
using BHYT.API.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BHYT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HealthHistoryController : ControllerBase
    {
        private readonly BHYTDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public HealthHistoryController(BHYTDbContext context, IConfiguration configuration, IMapper mapper)
        {
            _context = context;
            _configuration = configuration;
            _mapper = mapper;
        }

        [HttpGet("user/{id}")]

        public async Task<ActionResult<IEnumerable<HealthHistoryDTO>>> GetCustomerHealthHistory(int id)
        {
            try
            {
                var customerHealthHistories = await _context.HealthHistories
                 .Where(healthHistory => healthHistory.CustomerId == id)
                 .ToListAsync();

                if (customerHealthHistories != null)
                {
                    return Ok(_mapper.Map<List<HealthHistoryDTO>>(customerHealthHistories));
                }
                return NotFound(new ApiResponse { Message = "Không tìm lịch sử sức khỏe khách hàng !" });
            }
            catch (Exception)
            {
                return Conflict(new ApiResponseDTO
                {
                    Message = "Lỗi xảy ra lấy lịch sử sức khỏe khách hàng !"
                });
            }
        }

    }
}
