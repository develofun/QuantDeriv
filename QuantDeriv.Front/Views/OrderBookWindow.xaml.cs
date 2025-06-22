using System.Windows;

namespace QuantDeriv.Front
{
    /// <summary>
    /// OrderBookWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class OrderBookWindow : Window
    {
        public OrderBookWindow(OrderBookViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }
    }
}
