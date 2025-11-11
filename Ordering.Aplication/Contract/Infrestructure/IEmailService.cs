using Ordering.Application.Models;

namespace Ordering.Application.Contract.Infrestructure
{
    public interface IEmailService
    {
        Task<bool> SendEmail(Email email);
    }
}