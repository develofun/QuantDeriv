using QuantDeriv.Common.Models;

namespace QuantDeriv.Back.Interfaces
{
    public interface IOrderService
    {
        Task ExecuteOrderAsync(Order order, string userIdentifier);
    }
}
