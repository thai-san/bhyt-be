using AutoMapper;
using BHYT.API.Models.DbModels;
using BHYT.API.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BHYT.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class HealthIndicatorController : ControllerBase
    {
        private readonly BHYTDbContext _context;
        private readonly IMapper _mapper;

        public HealthIndicatorController(BHYTDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        [HttpGet]
        public async Task<ActionResult<HealthIndicatorDTO>> GetHealthIndicators(int userId)
        {
            try
            {
                var healthIndicators = await _context.HealthIndicators
               .Where(x => x.CustomerId == userId)
               .FirstOrDefaultAsync();

                if (healthIndicators == null)
                {
                    return NotFound(new ApiResponseDTO
                    {
                        Message = " không tìm thấy khách hàng !"
                    });
                }

                return Ok(_mapper.Map<HealthIndicatorDTO>(healthIndicators));

            }
            catch (Exception ex)
            {
                return NotFound(new ApiResponseDTO
                {
                    Message = " lỗi lấy thông tin chỉ số sức khỏe khách hàng !",
                });
            }
        }

        [HttpPost]
        public async Task<ActionResult> UpdateHealthIndicator(HealthIndicatorDTO dto)
        {
            try
            {
                var healthIndicator = await _context.HealthIndicators
               .Where(x => x.CustomerId == dto.CustomerId)
               .FirstOrDefaultAsync();

                if (healthIndicator == null)
                {
                    return NotFound(new ApiResponseDTO
                    {
                        Message = " không tìm thấy thông tin chỉ số sức khỏe !"
                    });
                }
                // Ánh xạ từ dto sang healthIndicator
                _mapper.Map(dto, healthIndicator);
                healthIndicator.LastestUpdate = DateTime.Now;

                _context.SaveChanges();

                return Ok(new ApiResponseDTO { Message = "Cập nhật thành công !" });

            }
            catch (Exception ex)
            {
                return NotFound(new ApiResponseDTO
                {
                    Message = " lỗi cập nhật thông tin chỉ số sức khỏe !",
                });
            }
        }

    }
}
