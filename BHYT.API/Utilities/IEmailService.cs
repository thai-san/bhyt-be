using AutoMapper.Internal;
using BHYT.API.Models.DTOs;

namespace BHYT.API.Utilities
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailDTO mailrequest);
    }
}
