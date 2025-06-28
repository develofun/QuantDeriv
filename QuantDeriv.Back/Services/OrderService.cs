using Microsoft.AspNetCore.SignalR;
using QuantDeriv.Back.Hubs;
using QuantDeriv.Back.Interfaces;
using QuantDeriv.Back.Repositories;
using QuantDeriv.Common.Enums;
using QuantDeriv.Common.Models;

namespace QuantDeriv.Back.Services
{
    /// <summary>
    /// 주문 처리 및 주문 관련 비즈니스
    /// </summary>
    public class OrderService: IOrderService
    {
        private readonly IOrderMatchService _orderMatchService;
        private readonly ITradeDataRepository _tradeDataRepository;
        private readonly IHubContext<TradeHubs> _hubContext;

        public OrderService(IOrderMatchService orderMatchService, ITradeDataRepository tradeDataRepository, IHubContext<TradeHubs> hubContext)
        {
            _orderMatchService = orderMatchService;
            _tradeDataRepository = tradeDataRepository;
            _hubContext = hubContext;
        }

        /// <summary>
        /// client에서 주문 요청을 받아 처리
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task ExecuteOrderAsync(Order order, string userIdentifier)
        {
            if (order == null || string.IsNullOrWhiteSpace(order.Ticker) || order.Type == OrderType.None)
            {
                throw new ArgumentException("Invalid order data provided.");
            }

            try
            {
                _orderMatchService.ProcessOrder(order);
                await SendOrderBookUpdateAsync(order.Ticker);
                await SendTradeHistoryUpdateAsync();
            }
            catch(Exception ex)
            {
                // 오류 로그 남김
                await _hubContext.Clients.User(userIdentifier ?? "")
                    .SendAsync("ReceiveError", $"Failed to process order for {order.Ticker}");
            }
        }

        #region 클라이언트에 푸시

        /// <summary>
        /// OrderBook 업데이트를 클라이언트에 푸시
        /// </summary>
        /// <param name="ticker"></param>
        /// <returns></returns>
        public async Task SendOrderBookUpdateAsync(string ticker)
        {
            var orderBookUpdate = _tradeDataRepository.GetOrderBookUpdate(ticker);
            if (orderBookUpdate != null)
            {
                await _hubContext.Clients.Group(ticker).SendAsync("ReceiveOrderBookUpdate", orderBookUpdate);
            }
        }

        /// <summary>
        /// 거래 내역 업데이트를 클라이언트에 푸시
        /// </summary>
        /// <returns></returns>
        public async Task SendTradeHistoryUpdateAsync()
        {
            await _hubContext.Clients.All.SendAsync("ReceiveTradeHistoryUpdate", _tradeDataRepository.GetTradeHistories().OrderByDescending(th => th.TradeTime));
        }

        #endregion
    }
}
