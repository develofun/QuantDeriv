using QuantDeriv.Common.Models;

namespace QuantDeriv.Back.Interfaces
{
    public interface IOrderMatchService
    {
        void ProcessOrder(Order newOrder);
    }
}
