using Futarapp.Models;

namespace Futarapp.Services
{
    public interface IEmailSender
    {
        void SendEmails(Message message);
    }
}
