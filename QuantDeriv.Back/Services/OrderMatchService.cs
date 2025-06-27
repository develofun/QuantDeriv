using Microsoft.AspNetCore.SignalR;
using QuantDeriv.Back.Hubs;
using QuantDeriv.Back.Interfaces;
using QuantDeriv.Back.Repositories;
using QuantDeriv.Common.Enums;
using QuantDeriv.Common.Models;
using System.Diagnostics;

namespace QuantDeriv.Back.Services
{
    public class OrderMatchService: IOrderMatchService
    {
        private readonly TradeDataRepository _tradeDataRepository;

        public OrderMatchService(TradeDataRepository tradeDataRepository)
        {
            _tradeDataRepository = tradeDataRepository;
        }

        // 주문을 처리하고 매칭
        public void ProcessOrder(Order newOrder)
        {
            if (newOrder == null || !newOrder.IsValid())
            {
                return;
            }

            try
            {
                var tradeSide = newOrder.Type == OrderType.Ask ? TradeSide.Sell : TradeSide.Buy;
                var lockObject = _tradeDataRepository.GetTickerLock(newOrder.Ticker);
                lock (lockObject)
                {
                    var orderBook = _tradeDataRepository.OrderBooks[newOrder.Ticker];

                    if (newOrder.Type == OrderType.Ask)
                    {
                        MatchWithBids(newOrder, orderBook);
                    }
                    else if (newOrder.Type == OrderType.Bid)
                    {
                        MatchWithAsks(newOrder, orderBook);
                    }
                }
            }
            catch(Exception ex)
            {
                // 오류 로그 남김
                throw new Exception($"Error processing order for ticker {newOrder.Ticker}: {ex.Message}", ex);
            }
        }

        // 매도 호가와 매수 호가 매칭
        // 매도 호가가 최대 매수 호가보다 낮거나 같을 때 최대 매수 호가부터 매도
        private void MatchWithBids(Order sellOrder, OrderBook book)
        {
            while (book.Bids.Count > 0 && sellOrder.Quantity > 0 && sellOrder.Price <= book.Bids.First().Key)
            {
                var highestPriceBid = book.Bids.First();
                var bidOrder = highestPriceBid.Value;

                int tradeQuantity = Math.Min(sellOrder.Quantity, bidOrder.Quantity);

                _tradeDataRepository.TradeHistories.Add(new TradeHistory(sellOrder.Ticker, TradeSide.Sell, bidOrder.Price, tradeQuantity, DateTime.UtcNow));

                sellOrder.Quantity -= tradeQuantity;
                bidOrder.Quantity -= tradeQuantity;

                if (bidOrder.Quantity == 0) book.Bids.Remove(highestPriceBid.Key);
            }

            if (sellOrder.Quantity > 0)
            {
                if (!book.Asks.TryGetValue(sellOrder.Price, out Order? storedSellOrder))
                {
                    book.Asks[sellOrder.Price] = sellOrder;
                } 
                else
                {
                    storedSellOrder.Quantity += sellOrder.Quantity;
                }
            }
        }

		// 매수 호가가 최소 매도 호가보다 높거나 같을 때 최소 매도 호가부터 매수
		private void MatchWithAsks(Order buyOrder, OrderBook book)
        {
            while (buyOrder.Quantity > 0 && book.Asks.Count > 0 && buyOrder.Price >= book.Asks.Last().Key)
            {
                var lowestPriceAsk = book.Asks.Last();
                var askOrder = lowestPriceAsk.Value;

                int tradeQuantity = Math.Min(buyOrder.Quantity, askOrder.Quantity);

                _tradeDataRepository.TradeHistories.Add(new TradeHistory(buyOrder.Ticker, TradeSide.Buy, askOrder.Price, tradeQuantity, DateTime.UtcNow));

                buyOrder.Quantity -= tradeQuantity;
                askOrder.Quantity -= tradeQuantity;

                if (askOrder.Quantity == 0) book.Asks.Remove(lowestPriceAsk.Key);
            }

            if (buyOrder.Quantity > 0)
            {
                if (!book.Bids.TryGetValue(buyOrder.Price, out Order? storedBuyOrder))
                {
                    book.Bids[buyOrder.Price] = buyOrder;
                }
                else
                {
                    storedBuyOrder.Quantity += buyOrder.Quantity;
                }
            }
        }
    }
}
