using AutoMapper;
using AutoMapper.QueryableExtensions;
using BHYT.API.Models.DbModels;
using BHYT.API.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BHYT.API.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerPolicyController : ControllerBase
    {
        private readonly BHYTDbContext _context;
        private readonly IMapper _mapper;

        public CustomerPolicyController(BHYTDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public IActionResult GetCustomerPolicy(int id)
        {
            try
            {
                var customerPolicy = _context.CustomerPolicies
                 .Where(policy => policy.Id == id)
                 .ProjectTo<CustomerPolicyDTO>(_mapper.ConfigurationProvider)
                 .FirstOrDefault();

                if (customerPolicy != null)
                {
                    return Ok(new
                    {
                        customerPolicy
                    });
                }
                return NotFound(new ApiResponse { Message = "Không tìm thấy chính sách" });
            }
            catch (Exception)
            {
                return Conflict(new ApiResponseDTO
                {
                    Message = "Lỗi xảy ra khi lấy chính sách"
                });
            }
        }

        [HttpDelete("reject")]
        public async Task<ActionResult> RejectInsurancePolicy(int policyId)
        {
            CustomerPolicy customerPolicy;
            try
            {
                customerPolicy = _context.CustomerPolicies.Where(x => x.Id == policyId).FirstOrDefault();
                if (customerPolicy == null)
                {
                    return Conflict(new ApiResponseDTO
                    {
                        Message = "Chính sách không tồn tại trong hệ thống !"
                    });

                }
                //_context.CustomerPolicies.Remove(customerPolicy);
                customerPolicy.Status = null;
                await _context.SaveChangesAsync();

                return Ok(new ApiResponseDTO
                {
                    Message = "Xoá chính sách thành công !"
                });
            }
            catch
            {
                return Conflict(new ApiResponseDTO
                {
                    Message = "Lỗi xoá thông tin chính sách!"
                });
            }
            finally
            {
                customerPolicy = null;
            }
        }

        [HttpPost("Issue")]
        public async Task<ActionResult> IssuePolicy([FromBody] InsurancePolicyIssueDTO policyIssue)
        {
            CustomerPolicy customerPolicy;
            try
            {
                customerPolicy = _context.CustomerPolicies.Where(x => x.Id == policyIssue.policyId).FirstOrDefault();
                if (customerPolicy == null)
                {
                    return Conflict(new ApiResponseDTO
                    {
                        Message = "Chính sách chưa tồn tại trong hệ thống, người dùng chưa đăng kí chính sách bảo hiểm !"
                    });

                }
                _mapper.Map(policyIssue, customerPolicy); // Ánh xạ từ DTO sang model

                customerPolicy.Status = true;
                // customerPolicy.PremiumAmount = tính toán ()
                // customerPolicy.DeductibleAmount = tính toán ()
                //insert thông tin phê duyệt

                var policyApproval = new PolicyApproval();
                policyApproval.PolicyId = policyIssue.policyId;
                policyApproval.ApprovalDate = DateTime.Now;
                policyApproval.StatusId = 1;
                policyApproval.Guid = new Guid();
                policyApproval.EmployeeId = (from user in _context.Users
                                             join account in _context.Accounts on user.AccountId equals account.Id
                                             where account.Username == User.FindFirstValue(ClaimTypes.Name)
                                             select user.Id).FirstOrDefault();
                _context.PolicyApprovals.Add(policyApproval);

                await _context.SaveChangesAsync();


                return Ok(new ApiResponseDTO
                {
                    Message = "Phát hành chính sách thành công !"
                });
            }
            catch
            {
                return Conflict(new ApiResponseDTO
                {
                    Message = "Lỗi phát hành chính sách bảo hiểm !"
                });
            }
            finally
            {
                customerPolicy = null;
            }
        }

        [HttpGet("list-policy")]
        public async Task<ActionResult<IEnumerable<CustomerPolicyDTO>>> GetAllPolicyOfUserById(int id)
        {
            try
            {

                var policies = await _context.CustomerPolicies
                              .Where(x => x.CustomerId == id)
                              .OrderBy(x => x.Id)
                              .ToListAsync();
                return Ok(_mapper.Map<List<CustomerPolicyDTO>>(policies));

            }
            catch (Exception ex)
            {
                return NotFound(new ApiResponseDTO
                {
                    Message = " lỗi lấy danh sách chính sách bảo hiểm của khách hàng ",
                });
            }
        }
    }
}


