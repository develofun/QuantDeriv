using Microsoft.Extensions.DependencyInjection;
using QuantDeriv.Front.Interfaces;
using QuantDeriv.Front.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace QuantDeriv.Front.Services
{
    /// <summary>
    /// 윈도우 관련 작업을 처리
    /// </summary>
    public class WindowService : IWindowService
    {
        private readonly IServiceProvider _serviceProvider;
        // Ticker별로 OrderBookViewModel을 하나만 유지하기 위한 캐시
        private readonly Dictionary<string, OrderBookViewModel> _activeOrderBookViewModels = new();

        public WindowService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task ShowOrderBookWindow(string ticker)
        {
            if (!_activeOrderBookViewModels.ContainsKey(ticker))
            {
                var vm = _serviceProvider.GetRequiredService<OrderBookViewModel>();
                await vm.InitializeAsync(ticker); // Ticker 초기화
                _activeOrderBookViewModels.Add(ticker, vm);
            }

            var window = new OrderBookWindow(_activeOrderBookViewModels[ticker]);
            window.Show();
        }

        public async Task ShowTradeHistoryWindow()
        {
            var vm = _serviceProvider.GetRequiredService<TradeHistoryViewModel>();
            await vm.Load();
            var window = new TradeHistoryWindow(vm);
            window.Show();
        }

        public void ShowPlaceOrderDialog(string ticker, int initialPrice)
        {
            var vm = App.ServiceProvider.GetRequiredService<OrderViewModel>();
            vm.Initialize(ticker, initialPrice);

            var dialog = new OrderDialog
            {
                DataContext = vm,
                Owner = Application.Current.MainWindow // 부모 창 설정
            };

            // 창 닫기
            vm.CloseAction = () => dialog.Close();

            dialog.ShowDialog();
        }
    }
}
