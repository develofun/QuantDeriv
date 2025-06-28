using System.Windows;
using System.Windows.Input;

namespace QuantDeriv.Front
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }

        private void ListBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is MainViewModel vm && vm.OpenOrderBookWindowCommand.CanExecute(null))
            {
                vm.OpenOrderBookWindowCommand.Execute(null);
            }
        }
    }
}