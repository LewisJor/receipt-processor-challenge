using System.ComponentModel.DataAnnotations;

namespace SimpleReceiptProcessor.Models;

/// <summary>
/// Using the Required Annotations to validate requirements at the http request level
/// </summary>
public class Receipt
{
    [Required] public string Retailer { get; set; }

    [Required] public DateTime PurchaseDate { get; set; }

    [Required] public TimeSpan PurchaseTime { get; set; }

    [Required] public string Total { get; set; }

    [Required] [MinLength(1)] public List<Item> Items { get; set; }
}

public class Item
{
    [Required] public string ShortDescription { get; set; }

    [Required] public string Price { get; set; }
}