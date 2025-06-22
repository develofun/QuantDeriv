using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantDeriv.Front.Interfaces
{
    public interface IWindowService
    {
        Task ShowOrderBookWindow(string ticker); 
        Task ShowTradeHistoryWindow();
        void ShowPlaceOrderDialog(string ticker, int initialPrice);
    }
}
