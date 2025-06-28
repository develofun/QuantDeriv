using Microsoft.AspNetCore.SignalR.Client;
using QuantDeriv.Common.Models;
using QuantDeriv.Front.Interfaces;

namespace QuantDeriv.Front.Services
{
    public class SignalRService: ISignalRService
    {
        private readonly HubConnection _connection;
        public event Action<OrderBookUpdate> OnOrderBookUpdate;
        public event Action<IEnumerable<TradeHistory>> TradeHistoryUpdated;

        public bool IsConnected => _connection.State == HubConnectionState.Connected;

        public SignalRService()
        {
            _connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5065/tradeHub")
                .WithAutomaticReconnect()
                .Build();

            _connection.On<OrderBookUpdate>("ReceiveOrderBookUpdate", update => OnOrderBookUpdate?.Invoke(update));

            _connection.On<IEnumerable<TradeHistory>>("ReceiveTradeHistoryUpdate", trades => TradeHistoryUpdated?.Invoke(trades));
        }

        /// <summary>
        /// 백엔드와 SignalR 연결
        /// </summary>
        /// <returns></returns>
        public async Task ConnectAsync()
        {
            if (IsConnected) return;
            await _connection.StartAsync();
        }

        /// <summary>
        /// Ticker 기준으로 SignalR 그룹에 Subscribe
        /// </summary>
        /// <param name="ticker"></param>
        /// <returns></returns>
        public async Task SubscribeToTickerAsync(string ticker) => await _connection.InvokeAsync("SubscribeToTicker", ticker);

        public async Task UnsubscribeFromTickerAsync(string ticker) => await _connection.InvokeAsync("UnsubscribeFromTicker", ticker);

        /// <summary>
        /// 사용 가능한 Ticker 목록 조회
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetAvailableTickersAsync() => await _connection.InvokeAsync<IEnumerable<string>>("GetTickers");

        /// <summary>
        /// 선택한 ticker의 OrderBook 정보 조회
        /// </summary>
        /// <param name="ticker"></param>
        /// <returns></returns>
        public async Task<OrderBookUpdate> GetOrderBookAsync(string ticker) => await _connection.InvokeAsync<OrderBookUpdate>("GetOrderBook", ticker);

        /// <summary>
        /// 거래 내역 조회
        /// </summary>
        /// <returns></returns>
        public async Task<IList<TradeHistory>> GetTradeHistoryAsync() => await _connection.InvokeAsync<IList<TradeHistory>>("GetTradeHistory");
        
        /// <summary>
        /// 주문 등록 요청
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public async Task PlaceOrderAsync(Order order) => await _connection.InvokeAsync("PlaceOrder", order);
    }
}
