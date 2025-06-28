using QuantDeriv.Common.Models;

namespace QuantDeriv.Back.Interfaces
{
    public interface ITradeDataRepository
    {
        public IEnumerable<string> GetTickers();
        public object GetTickerLock(string ticker);
        public OrderBook GetOrderBook(string ticker);
        public OrderBookUpdate GetOrderBookUpdate(string ticker);
        public void AddOrderBook(string ticker, OrderBook orderBook);
        public void AddTradeHistory(TradeHistory tradeHistory);
        public IEnumerable<TradeHistory> GetTradeHistories(string ticker = "");
    }
}
