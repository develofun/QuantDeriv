namespace QuantDeriv.Front.Interfaces
{
    public interface IWindowService
    {
        Task ShowOrderBookWindow(string ticker); 
        Task ShowTradeHistoryWindow();
        void ShowPlaceOrderDialog(string ticker, int initialPrice);
    }
}
