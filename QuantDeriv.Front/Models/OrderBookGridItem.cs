namespace QuantDeriv.Front.Models
{
    //public class OrderBookGridItem
    //{
    //    public string Layer { get; set; }  // "ask10" ~ "ask1" / "bid1" ~ "bid10"
    //    public int? Price { get; set; } // 가격 (구분선은 null)
    //    public int? Quantity { get; set; } // 수량
    //    public string RowType { get; set; } // "Ask", "Bid" 구분
    //}

    public record OrderBookGridItem(
        string Layer, 
        int? Price, 
        int? Quantity, 
        string RowType
    );
}
