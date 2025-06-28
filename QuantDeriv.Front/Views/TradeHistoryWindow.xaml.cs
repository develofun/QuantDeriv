using System.Windows;

namespace QuantDeriv.Front
{
    /// <summary>
    /// TradeHistoryWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TradeHistoryWindow : Window
    {
        public TradeHistoryWindow(TradeHistoryViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }
    }
}
