using QuantDeriv.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantDeriv.Front.Interfaces
{
    public interface ISignalRService
    {
        event Action<OrderBookUpdate> OnOrderBookUpdate;
        event Action<IEnumerable<TradeHistory>> TradeHistoryUpdated;
        Task ConnectAsync();
        Task SubscribeToTickerAsync(string ticker);
        Task<IEnumerable<string>> GetAvailableTickersAsync();
        Task<OrderBookUpdate> GetOrderBookAsync(string ticker);
        Task<IList<TradeHistory>> GetTradeHistoryAsync();
        Task PlaceOrderAsync(Order order);
    }
}
