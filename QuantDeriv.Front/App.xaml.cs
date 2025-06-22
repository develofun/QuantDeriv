using Microsoft.Extensions.DependencyInjection;
using QuantDeriv.Front.Interfaces;
using QuantDeriv.Front.Services;
using System.Windows;

namespace QuantDeriv.Front
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        public App()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {

            try
            {
                // 1. 먼저 MainViewModel 인스턴스를 DI 컨테이너로부터 가져옵니다.
                var mainViewModel = ServiceProvider.GetRequiredService<MainViewModel>();

                // 2. ViewModel의 비동기 초기화 메서드를 명시적으로 'await' 합니다.
                await mainViewModel.InitializeAsync();

                // 3. 이제 모든 데이터가 준비된 ViewModel을 사용하여 MainWindow를 생성합니다.
                //    DI 컨테이너에게 ViewModel을 직접 전달하여 MainWindow를 만듭니다.
                var mainWindow = new MainWindow(mainViewModel);
                mainWindow.Show();
            }
            catch (Exception ex)
            {
                // 초기화 과정에서 발생하는 모든 예외를 잡아서 사용자에게 보여줍니다.
                MessageBox.Show($"애플리케이션을 시작하는 중 심각한 오류가 발생했습니다.\n\n오류: {ex.Message}", "시작 오류", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<ISignalRService, SignalRService>();
            services.AddSingleton<IWindowService, WindowService>();
            services.AddTransient<MainViewModel>();
            services.AddTransient<OrderBookViewModel>();
            services.AddTransient<OrderViewModel>();
            services.AddTransient<TradeHistoryViewModel>();
            services.AddTransient<MainWindow>();
        }
    }

}
