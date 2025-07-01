using DoeTech.DTOs;

namespace DoeTech.Services;

public interface IEmailService : IService
{
    Task SendEmail(EmailPayload payload);
}