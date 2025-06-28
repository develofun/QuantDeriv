using Microsoft.AspNetCore.SignalR;
using QuantDeriv.Back.Interfaces;
using QuantDeriv.Back.Repositories;
using QuantDeriv.Common.Models;

namespace QuantDeriv.Back.Hubs
{
    public class TradeHubs: Hub
    {
        private readonly IOrderService _orderService;
        private readonly TradeDataRepository _tradeDataRepository;

        public TradeHubs(IOrderService orderService, TradeDataRepository tradeDataRepository)
        {
            _orderService = orderService;
            _tradeDataRepository = tradeDataRepository;
        }

        #region SignalR 그룹 관리

        public async Task SubscribeToTicker(string ticker)
        {
            if (string.IsNullOrEmpty(ticker))
            {
                throw new ArgumentException("Ticker cannot be null or empty.", nameof(ticker));
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, ticker);
        }

        public async Task UnsubscribeFromTicker(string ticker)
        {
            if (string.IsNullOrEmpty(ticker))
            {
                throw new ArgumentException("Ticker cannot be null or empty.", nameof(ticker));
            }

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, ticker);
        }

        #endregion

        #region 클라이언트로부터 호출

        public async Task PlaceOrder(Order order)
        {
            await _orderService.ExecuteOrderAsync(order, Context.UserIdentifier ?? "");
        }

        public IEnumerable<string> GetTickers()
        {
            return _tradeDataRepository.GetTickers();
        }

        public OrderBookUpdate GetOrderBook(string ticker)
        {
            return _tradeDataRepository.GetOrderBookUpdate(ticker);
        }

        public IEnumerable<TradeHistory> GetTradeHistory()
        {
            return _tradeDataRepository.GetTradeHistories();
        }

        #endregion
    }
}
