using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuantDeriv.Front.Interfaces;
using System.Collections.ObjectModel;

namespace QuantDeriv.Front
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly IWindowService _windowService;
        private readonly ISignalRService _signalRService;
        public ObservableCollection<string> Tickers { get; } = new();
        [ObservableProperty] private string _selectedTicker;

        public MainViewModel(IWindowService windowService, ISignalRService signalRService)
        {
            _windowService = windowService;
            _signalRService = signalRService;
        }

        public async Task InitializeAsync()
        {
            // SignalR 연결도 ViewModel 초기화의 일부로 이동
            await _signalRService.ConnectAsync();
            var tickers = await _signalRService.GetAvailableTickersAsync();

            foreach (var ticker in tickers)
            {
                Tickers.Add(ticker);
            }
        }

        [RelayCommand]
        private void OpenOrderBookWindow()
        {
            if (!string.IsNullOrEmpty(SelectedTicker)) _windowService.ShowOrderBookWindow(SelectedTicker);
        }

        [RelayCommand]
        private void OpenTradeHistory() => _windowService.ShowTradeHistoryWindow();
    }
}
