using System;
namespace QuantDeriv.Common.Models
{
	// backend에서 사용하는 주문서 정보로 common에 정의 안해도 됨
	public class OrderBook
    {
        public SortedDictionary<int, Order> Asks { get; set; } = new(Comparer<int>.Create((a, b) => b.CompareTo(a)));

        public SortedDictionary<int, Order> Bids { get; set; } = new(Comparer<int>.Create((a, b) => b.CompareTo(a)));
    }

	// 클라에 전달되는 주문서 업데이트 정보
	public class OrderBookUpdate
    {
        public OrderBookUpdate(string ticker, IEnumerable<Order> asks, IEnumerable<Order> bids)
        {
            Ticker = ticker;
            Asks = asks;
            Bids = bids;
        }

        public string Ticker { get; set; }

        public IEnumerable<Order> Asks { get; set; }

        public IEnumerable<Order> Bids { get; set; }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Ticker) && Asks.Any() && Bids.Any();
        }
    }
}
