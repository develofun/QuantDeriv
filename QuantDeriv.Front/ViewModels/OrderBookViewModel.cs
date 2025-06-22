using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuantDeriv.Common.Enums;
using QuantDeriv.Common.Models;
using QuantDeriv.Front.Interfaces;
using QuantDeriv.Front.Models;
using QuantDeriv.Front.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace QuantDeriv.Front
{
    public partial class OrderBookViewModel : ObservableObject
    {
        private readonly IWindowService _windowService;
        private readonly ISignalRService _signalRService;

        [ObservableProperty] public string _ticker;

        public ObservableCollection<Order> Asks { get; } = new();
        public ObservableCollection<Order> Bids { get; } = new();
        public ObservableCollection<OrderBookGridItem> OrderBookSource { get; } = new();

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

            Application.Current.Dispatcher.Invoke(() =>
            {
                OrderBookSource.Clear();

                // 1. 매도(Asks) 리스트 가공
                // 서버에서 받은 Asks는 가격 오름차순 (ask1, ask2, ...)
                // 화면에는 ask10..ask1 순으로 보여주기 위해 역순으로 처리
                var asksToDisplay = update.Asks.Take(10).ToList();
                var asksCount = asksToDisplay.Count();
                for (int i = 0; i < asksCount; i++)
                {
                    var ask = update.Asks.ElementAt(i);
                    string layer = $"Ask {asksCount - i}";
                    OrderBookSource.Add(new OrderBookGridItem
                    {
                        Layer = layer,
                        Price = ask.Price,
                        Quantity = ask.Quantity,
                        RowType = "Ask"
                    });
                }

                // 2. 중간 구분선 추가
                if (update.Asks.Any() && update.Bids.Any())
                {
                    OrderBookSource.Add(new OrderBookGridItem { RowType = "Spread" });
                }

                // 3. 매수(Bids) 리스트 가공
                // 서버에서 받은 Bids는 가격 내림차순 (bid1, bid2, ...)
                var bidsToDisplay = update.Bids.Take(10).ToList();
                var bidsCount = bidsToDisplay.Count();
                for (int i = 0; i < bidsCount; i++)
                {
                    var bid = update.Bids.ElementAt(i);
                    // 리스트의 위에서부터 bid1, bid2, ... 로 표시
                    string layer = $"Bid {i + 1}";
                    OrderBookSource.Add(new OrderBookGridItem
                    {
                        Layer = layer,
                        Price = bid.Price,
                        Quantity = bid.Quantity,
                        RowType = "Bid"
                    });
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
            var bestAsk = OrderBookSource.FirstOrDefault(i => i.RowType == "Ask")?.Price;
            var bestBid = OrderBookSource.FirstOrDefault(i => i.RowType == "Bid")?.Price;
            if (bestAsk.HasValue && bestBid.HasValue)
            {
                initialPrice = (bestAsk.Value + bestBid.Value) / 2;
            }

            _windowService.ShowPlaceOrderDialog(Ticker, initialPrice);
        }
    }
}
