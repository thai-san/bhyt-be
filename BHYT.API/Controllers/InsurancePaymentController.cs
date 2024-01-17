using AutoMapper;
using AutoMapper.QueryableExtensions;
using BHYT.API.Models.DbModels;
using BHYT.API.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BHYT.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class InsurancePaymentController : ControllerBase
    {
        private readonly BHYTDbContext _context;
        private readonly IMapper _mapper;

        public InsurancePaymentController(BHYTDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public IActionResult GetUserInsurancePayment(int id)
        {
            try
            {
                var insurancePayments = _context.InsurancePayments
                       .Where(payment => payment.CustomerId == id)
                       .ProjectTo<InsurancePaymentDTO>(_mapper.ConfigurationProvider)
                       .ToList();

                if (insurancePayments.Any())
                {
                    return Ok(new
                    {
                        insurancePayments
                    });
                }
                return NotFound(new ApiResponse { Message = "Không tìm thấy yêu cầu thanh toán của khách hàng này" });
            }
            catch (Exception)
            {
                return Conflict(new ApiResponseDTO
                {
                    Message = "Lỗi xảy ra khi lấy yêu câu thanh toán"
                });
            }
        }

        [HttpGet]
        public IActionResult GetAllUserInsurancePayments()
        {
            try
            {
                var insurancePayments = _context.InsurancePayments
                       .ProjectTo<InsurancePaymentDTO>(_mapper.ConfigurationProvider)
                       .ToList();

                if (insurancePayments.Any())
                {
                    return Ok(new
                    {
                        insurancePayments
                    });
                }
                return NotFound(new ApiResponse { Message = "Không tìm thấy yêu cầu thanh toán nào" });
            }
            catch (Exception)
            {
                return Conflict(new ApiResponseDTO
                {
                    Message = "Lỗi xảy ra khi lấy yêu câu thanh toán"
                });
            }
        }

        [HttpDelete("{Guid}")]
        public IActionResult DeleteUserInsurancePayment(Guid Guid)
        {
            try
            {
                var insurancePayment = _context.InsurancePayments.FirstOrDefault(payment => payment.Guid == Guid);

                if (insurancePayment == null)
                {
                    return NotFound(new ApiResponse { Message = "Không tìm thấy yêu cầu thanh toán của khách hàng này" });
                }

                _context.InsurancePayments.Remove(insurancePayment);
                _context.SaveChanges();

                return Ok(new ApiResponse { Message = "Yêu cầu thanh toán đã được xóa thành công" });
            }
            catch (Exception)
            {
                return Conflict(new ApiResponseDTO
                {
                    Message = "Lỗi xảy ra khi xóa yêu câu thanh toán"
                });
            }
        }

        [HttpPost("{customerId}")]
        public IActionResult CreateInsurancePayment(int customerId , [FromBody] InsurancePaymentDTO insurancePaymentDTO)
        {
            try
            {
                var insurancePayment = new InsurancePayment
                {   
                    Guid = Guid.NewGuid(),
                    CustomerId = customerId,
                    PolicyId = insurancePaymentDTO.PolicyId,
                    Date = DateTime.Now,
                    Amount = insurancePaymentDTO.Amount,
                    Status = insurancePaymentDTO.Status,
                    Type = insurancePaymentDTO.Type,
                    Note = insurancePaymentDTO.Note
                };

                _context.InsurancePayments.Add(insurancePayment);
                _context.SaveChanges();

                return Ok(new ApiResponse { Message = "Yêu cầu thanh toán đã được tạo thành công" });
            }
            catch (Exception)
            {
                return Conflict(new ApiResponseDTO
                {
                    Message = "Lỗi xảy ra khi tạo yêu câu thanh toán"
                });
            }
        }
    }
}
