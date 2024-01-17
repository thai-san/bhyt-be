using AutoMapper;
using BHYT.API.Models.DbModels;
using BHYT.API.Models.DTOs;

namespace BHYT.API.Utilities
{
    public class AutoMapper: Profile
    {
        public AutoMapper() {
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<InsurancePayment, InsurancePaymentDTO>().ReverseMap();
            CreateMap<CustomerPolicy, CustomerPolicyDTO>().ReverseMap();
            CreateMap<HealthHistory, HealthHistoryDTO>().ReverseMap();
            CreateMap<CustomerPolicy, InsurancePolicyIssueDTO>().ReverseMap()
                .ForMember(dest => dest.CoverageType, opt => opt.MapFrom(src => "Bảo hiểm sức khỏe cơ bản")) // Gán giá trị mặc định cho CoverageType
                .ForMember(dest => dest.LatestUpdate, opt => opt.MapFrom(src => DateTime.Now)) // Gán giá trị hiện tại cho LatestUpdate
                .ForMember(dest => dest.Company, opt => opt.MapFrom(src => "ABC Insurance"));

        }
    }
}
