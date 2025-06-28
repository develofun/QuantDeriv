using QuantDeriv.Common.Enums;

namespace QuantDeriv.Common.Models
{
    public class Order
    {
        public Order()
        {
            Ticker = string.Empty;
            Type = OrderType.None;
            Price = 0;
            Quantity = 0;
        }

        public Order(string ticker, OrderType type, int price, int quantity)
        {
            Ticker = ticker;
            Type = type;
            Price = price;
            Quantity = quantity;
        }

        public string Ticker { get; set; }

        /// <summary>
        /// Bid or Ask Layer
        /// </summary>
        public OrderType Type { get; set; }

        /// <summary>
        /// Ask/Bid Price
        /// </summary>
        public int Price { get; set; }
        
        /// <summary>
        /// Ask/Bid Quantity
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 주문 시간
        /// </summary>
        public DateTime OrderTime { get; set; }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Ticker) && Price > 0 && Quantity > 0;
        }
    }
}
