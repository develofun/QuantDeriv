using QuantDeriv.Back.Interfaces;
using QuantDeriv.Back.Repositories;
using QuantDeriv.Common.Enums;
using QuantDeriv.Common.Models;

namespace QuantDeriv.Back.Services
{
    /// <summary>
    /// 매매 테스트를 위한 자동 매매 시뮬레이션
    /// </summary>
    public class TradeSimulator : BackgroundService
    {
        private readonly ITradeDataRepository _tradeDataRepository;
        private readonly IOrderService _orderService;
        private readonly Random _random = new();

        public TradeSimulator(ITradeDataRepository tradeDataRepository, IOrderService orderService)
        {
            _tradeDataRepository = tradeDataRepository;
            _orderService = orderService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // 초기 시스템 대기 시간
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken); 

            var tickers = _tradeDataRepository.GetTickers();
            var tickerCount = tickers.Count();

            while (!stoppingToken.IsCancellationRequested)
            {
                var ticker = tickers.ElementAt(_random.Next(tickerCount));

                OrderType type = _random.Next(0, 2) == 0 ? OrderType.Ask : OrderType.Bid;
                int price = type == OrderType.Ask ? _random.Next(40, 100) : _random.Next(1, 60);
                var quantity = _random.Next(1, 20);

                var order = new Order(ticker, type, price, quantity);
                await _orderService.ExecuteOrderAsync(order, "");

                await Task.Delay(TimeSpan.FromMilliseconds(500), stoppingToken);
            }
        }
    }
}
