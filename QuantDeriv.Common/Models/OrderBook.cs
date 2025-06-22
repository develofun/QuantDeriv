using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace QuantDeriv.Common.Models
{
    public class OrderBook
    {
        public SortedDictionary<int, Order> Asks { get; set; } = new(Comparer<int>.Create((a, b) => a.CompareTo(b)));

        public SortedDictionary<int, Order> Bids { get; set; } = new(Comparer<int>.Create((a, b) => b.CompareTo(a)));
    }

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
