using System.Threading.Tasks;

namespace Rased.Core.ServiseContracts
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string phoneNumber, string message);
    }
}
