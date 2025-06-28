using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuantDeriv.Common.Enums;
using QuantDeriv.Common.Models;
using QuantDeriv.Front.Interfaces;
using QuantDeriv.Front.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace QuantDeriv.Front
{
    public partial class OrderBookViewModel : ObservableObject
    {
        private readonly IWindowService _windowService;
        private readonly ISignalRService _signalRService;

        [ObservableProperty] private string _ticker;

        public IList<Order> Asks { get; } = new ObservableCollection<Order>();
        public IList<Order> Bids { get; } = new ObservableCollection<Order>();
        public IList<OrderBookGridItem> OrderBookSource { get; } = new ObservableCollection<OrderBookGridItem>();
        private List<OrderBookGridItem> _orderBookGridItems = [];

        public List<string> OrderTypes { get; } = Enum.GetNames(typeof(OrderType)).ToList();

        [ObservableProperty] private string _selectedOrderType = OrderType.Ask.ToString();
        [ObservableProperty] private bool _isOrderPanelVisible = false;

        public OrderBookViewModel(IWindowService windowService, ISignalRService signalRService)
        {
            _windowService = windowService;
            _signalRService = signalRService;
            _signalRService.OnOrderBookUpdate += UpdateOrderBook;
        }

        public async Task InitializeAsync(string ticker)
        {
            Ticker = ticker;
            await _signalRService.SubscribeToTickerAsync(ticker);

            var initialOrderBook = await _signalRService.GetOrderBookAsync(ticker);
            if (initialOrderBook != null)
            {
                // 받아온 정보로 UI 업데이트
                UpdateOrderBook(initialOrderBook);
            }
        }

        private void UpdateOrderBook(OrderBookUpdate update)
        {
            if (update.Ticker != Ticker) return;

            _orderBookGridItems.Clear();

            // 1. 매도(Asks) 리스트 가공
            var asksCount = update.Asks.Count();
            _orderBookGridItems.AddRange(update.Asks.Select((ask, idx) => new OrderBookGridItem($"Ask {asksCount - idx}", ask.Price, ask.Quantity, "Ask")));

            // 2. 중간 구분선 추가
            if (update.Asks.Any() && update.Bids.Any())
            {
                _orderBookGridItems.Add(new OrderBookGridItem("", 0, 0, "Spread"));
            }

            // 3. 매수(Bids) 리스트 가공
            // 서버에서 받은 Bids는 가격 내림차순 (bid1, bid2, ...)
            var bidsCount = update.Bids.Count();
            _orderBookGridItems.AddRange(update.Bids.Select((bid, idx) => new OrderBookGridItem($"Bid {bidsCount - idx}", bid.Price, bid.Quantity, "Bid")));

            Application.Current.Dispatcher.Invoke(() =>
            {
                OrderBookSource.Clear();

                foreach(var item in _orderBookGridItems)
                {
                    OrderBookSource.Add(item);
                }
            });
        }

        [RelayCommand]
        private void ToggleOrderPanel()
        {
            IsOrderPanelVisible = !IsOrderPanelVisible;
        }

        [RelayCommand]
        private void ShowOrderDialog()
        {
            // 현재 호가창의 중간 가격을 초기 주문 가격으로 전달
            int initialPrice = 0;
            var bestAsk = OrderBookSource.LastOrDefault(i => i.RowType == "Ask")?.Price;
            var bestBid = OrderBookSource.FirstOrDefault(i => i.RowType == "Bid")?.Price;
            if (bestAsk.HasValue && bestBid.HasValue)
            {
                initialPrice = (bestAsk.Value + bestBid.Value) / 2;
            }

            _windowService.ShowPlaceOrderDialog(Ticker, initialPrice);
        }
    }
}
