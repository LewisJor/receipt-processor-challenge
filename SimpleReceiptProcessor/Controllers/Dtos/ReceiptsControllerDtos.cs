namespace SimpleReceiptProcessor.Controllers.Dtos;

public class ReceiptDto
{
    public string Retailer { get; set; }
    public DateTime PurchaseDate { get; set; }
    public TimeSpan PurchaseTime { get; set; }
    public List<ItemDto> Items { get; set; }
    public string Total { get; set; }
}

public class ItemDto
{
    public string ShortDescription { get; set; }
    public string Price { get; set; }
}