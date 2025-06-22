using QuantDeriv.Common.Enums;
using QuantDeriv.Common.Models;
using System.Collections.Concurrent;

namespace QuantDeriv.Back.Repositories
{
    /// <summary>
    /// 티커 및 거래 데이터 관리 Repository
    /// </summary>
    public class TradeDataRepository
    {
        // 샘플 데이터 생성용 constants
        private const int MaxSampleTickerCount = 20;
        private const int MaxSampleOrderBookCount = 10;
        private const int MaxSampleHistoryCount = 10;

        public IList<string> Tickers { get; } = [];

        // 병렬처리 및 thread safe를 위한 Concurrent collections 사용
        // { get; } 참조 추적
        public ConcurrentBag<TradeHistory> TradeHistories { get; } = new();
        public ConcurrentDictionary<string, OrderBook> OrderBooks { get; } = new();

        // 데이터 무결성을 위한 TickerLocks
        private readonly ConcurrentDictionary<string, object> TickerLocks = new();

        public TradeDataRepository()
        {
            Random random = new Random();
            GenerateRandomTicker(random);
            AddSampleOrderBookData(random);
            GenerateSampleTradeHistories(random);
        }

        // 랜덤 티커 생성 (4~5글자 조합)
        // 예: AAPL, MSFT, GOOGL 등
        // 26개의 알파벳을 사용하여 4 ~ 5글자 조합의 티커를 생성
        private void GenerateRandomTicker(Random random)
        {
            for (int i = 0; i < MaxSampleTickerCount; i++)
            {
                int length = random.Next(4, 6); // 4 또는 5글자
                char[] tickerChars = new char[length];
                for (int j = 0; j < length; j++)
                {
                    tickerChars[j] = (char)('A' + random.Next(0, 26)); // A-Z 사이의 랜덤 문자
                }

                var ticker = new string(tickerChars);
                Tickers.Add(ticker);
                OrderBooks.TryAdd(ticker, new OrderBook());
                TickerLocks.TryAdd(ticker, new object());
            }
        }

        // 티커별로 orderbook에 ask, bid 10개씩 추가
        // orderbook의 price를 ask는 40 ~ 100, bid는 1 ~ 60 사이의 랜덤값으로 설정
        // quantity는 1 ~ 100 사이의 랜덤값으로 설정
        private void AddSampleOrderBookData(Random random)
        {
            foreach (var ticker in Tickers)
            {
                var orderBook = OrderBooks[ticker];
                for (int i = 0; i < MaxSampleOrderBookCount; i++)
                {
                    var askPrice = random.Next(40, 100);
                    // Ask 가격이 이미 존재하지 않는 경우에만 추가 (중복 방지)
                    if (!orderBook.Asks.ContainsKey(askPrice))
                    {
                        orderBook.Asks.Add(askPrice, new Order(ticker, OrderType.Ask, askPrice, random.Next(1, 20)));
                    }

                    var bidPrice = random.Next(1, 60);
                    // Bid 가격이 이미 존재하지 않는 경우에만 추가 (중복 방지)
                    if (!orderBook.Bids.ContainsKey(bidPrice))
                    {
                        orderBook.Bids.Add(bidPrice, new Order(ticker, OrderType.Bid, bidPrice, random.Next(1, 20)));
                    }
                }
            }            
        }

        // 생성된 티커 리스트 기준으로 TradeHistory를 생성
        // TradeHistory는 랜덤하게 10개 생성
        public void GenerateSampleTradeHistories(Random random)
        {
            for (int i = 0; i < MaxSampleHistoryCount; i++)
            {
                var ticker = Tickers[random.Next(Tickers.Count)];
                var price = random.Next(1, 100);
                var quantity = random.Next(1, 20);
                TradeHistories.Add(new TradeHistory(ticker, random.Next(0, 1) == 0 ? TradeSide.Buy : TradeSide.Sell, price, quantity, DateTime.UtcNow.AddSeconds(-random.Next(0, 3600))));
            }
        }

        // 티커별로 Lock 객체를 생성하여 동시성 문제를 방지
        public object GetTickerLock(string ticker)
        {
            return TickerLocks.GetOrAdd(ticker, t => new object());
        }

        // 클라이언트에 전달할 OrderBookUpdate 객체 생성
        public OrderBookUpdate GetOrderBookUpdate(string ticker)
        {
            if (OrderBooks.TryGetValue(ticker, out var orderBook))
            {
                return new OrderBookUpdate(
                    ticker,
                    orderBook.Asks.Values.Take(10).OrderByDescending(x => x.Price),
                    orderBook.Bids.Values.Take(10)
                );
            }

            return null;
        }

    }
}
