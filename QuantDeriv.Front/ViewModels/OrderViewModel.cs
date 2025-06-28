using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuantDeriv.Common.Models;
using QuantDeriv.Front.Interfaces;
using System.Windows;

namespace QuantDeriv.Front
{
    public partial class OrderViewModel: ObservableObject
    {
        private readonly ISignalRService _signalRService;

        public Action CloseAction { get; set; }

        [ObservableProperty] private string _ticker;
        [ObservableProperty] private int _orderPrice;
        [ObservableProperty] private int _orderQuantity = 1;
        [ObservableProperty] private string _selectedOrderType = "Ask";

        public List<string> OrderTypes { get; } = new List<string> { "Ask", "Bid" };

        public OrderViewModel(ISignalRService signalRService)
        {
            _signalRService = signalRService;
        }

        public void Initialize(string ticker, int initialPrice)
        {
            Ticker = ticker;
            OrderPrice = initialPrice;
        }

        [RelayCommand]
        public async Task SubmitOrder()
        {
            if (string.IsNullOrEmpty(Ticker) || OrderPrice <= 0 || OrderQuantity <= 0)
            {
                MessageBox.Show("주문 가격과 수량은 0보다 커야 합니다.", "주문 오류");
                return;
            }

            var orderType = SelectedOrderType == "Ask" ? Common.Enums.OrderType.Ask : Common.Enums.OrderType.Bid;
            var order = new Order(Ticker, orderType, OrderPrice, OrderQuantity);
            await _signalRService.PlaceOrderAsync(order);
            CloseAction?.Invoke();
        }
    }
}
