using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantDeriv.Common.Models
{
    public class TickerData
    {
        public TickerData(string symbol)
        {
            Symbol = symbol;
            Price = 0;
            LastQuantity = 0;
        }

        public TickerData() => new TickerData(string.Empty);

        /// <summary>
        /// Ticker Symbol
        /// </summary>
        public string Symbol { get; set; }

        public int Price { get; set; }

        public int LastQuantity { get; set; }
    }
}
