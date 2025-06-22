using QuantDeriv.Common.Enums;

namespace QuantDeriv.Common.Models
{
    public class TradeHistory
    {
        public TradeHistory(string ticker, TradeSide side, int price, int quantity, DateTime tradeTime)
        {
            Ticker = ticker;
            Side = side;
            Price = price;
            Quantity = quantity;
            TradeTime = tradeTime;
        }

        /// <summary>
        /// Ticker
        /// </summary>
        public string Ticker { get; set; }

        /// <summary>
        /// Buy or Sell
        /// </summary>
        public TradeSide Side { get; set; }

        /// <summary>
        /// Trade Price
        /// </summary>
        public int Price { get; set; }

        /// <summary>
        /// Trade Quantity
        /// </summary>
        public int Quantity { get; set; }

        public DateTime TradeTime { get; set; }
    }
}
