using Microsoft.Extensions.DependencyInjection;
using QuantDeriv.Front.Interfaces;
using QuantDeriv.Front.Views;
using System.Windows;

namespace QuantDeriv.Front.Services
{
    /// <summary>
    /// 윈도우 관련 작업을 처리
    /// viewmodel 인스턴스를 생성하고, 창을 표시하는 등의 작업을 수행
    /// </summary>
    public class WindowService : IWindowService
    {
        private readonly IServiceProvider _serviceProvider;
        // Ticker별로 OrderBookViewModel 인스턴스를 하나만 유지하기 위한 캐시
        private readonly Dictionary<string, OrderBookViewModel> _activeOrderBookViewModels = new();

        public WindowService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task ShowOrderBookWindow(string ticker)
        {
            if (!_activeOrderBookViewModels.ContainsKey(ticker))
            {
                // 서비스 프로바이더를 통해 OrderBookViewModel 인스턴스를 가져옴
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
