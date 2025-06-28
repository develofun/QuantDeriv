using CommunityToolkit.Mvvm.ComponentModel;
using QuantDeriv.Common.Models;
using QuantDeriv.Front.Interfaces;
using System.Collections.ObjectModel;
using System.Windows;

namespace QuantDeriv.Front
{
    public partial class TradeHistoryViewModel: ObservableObject
    {
        private readonly ISignalRService _signalRService;
        public ObservableCollection<TradeHistory> Trades { get; } = new();

        public TradeHistoryViewModel(ISignalRService signalRService)
        {
            _signalRService = signalRService;
            _signalRService.TradeHistoryUpdated += UpdateTradeHistory;
        }

        public async Task Load()
        {
            var result = await _signalRService.GetTradeHistoryAsync();
            if (result == null) return;
            Trades.Clear();
            foreach (var item in result) {
                Trades.Add(item);
            }
        }

        private void UpdateTradeHistory(IEnumerable<TradeHistory> tradeHistories)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Trades.Clear();
                foreach (var item in tradeHistories)
                {
                    Trades.Add(item);
                }
            });
        }
    }
}
