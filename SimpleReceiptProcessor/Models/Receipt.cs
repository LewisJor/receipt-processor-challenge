using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using SimpleReceiptProcessor.Controllers.Converters;

namespace SimpleReceiptProcessor.Models;

/// <summary>
/// The User provided receipt. Unsure if we want to reject any receipts that don't match the patterns.
/// If not we can apply regex in the request.
/// </summary>
public class Receipt
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// The name of the retailer or store the receipt is from.
    /// </summary>
    /// <remarks>
    /// pattern "^[\\w\\s\\-&]+$"
    /// example: "M&M Corner Market"
    /// </remarks>
    [Required]
    public string Retailer { get; set; }

    /// <summary>
    /// The date of the purchase printed on the receipt.
    /// </summary>
    /// <remarks>
    /// example: "2022-01-01"
    /// </remarks>
    [Required]
    [DataType(DataType.Date)]
    public DateTime PurchaseDate { get; set; }

    /// <summary>
    /// The time of the purchase printed on the receipt. 24-hour time expected
    /// </summary>
    /// <remarks>
    /// example: "13:01"
    /// </remarks>
    [Required]
    [DataType(DataType.Time)]
    [JsonConverter(typeof(TimeSpanConverter))]
    public TimeSpan PurchaseTime { get; set; }

    /// <summary>
    /// The list of <see cref="Item"/> purchased
    /// </summary>
    /// <remarks>
    /// Minimum length of 1
    /// </remarks>
    [Required]
    public ICollection<Item> Items { get; set; } = new List<Item>();

    /// <summary>
    /// The total amount paid on the receipt
    /// </summary>
    /// <remarks>
    /// pattern "^\\d+\\.\\d{2}$"
    /// example: "6.49"
    /// </remarks>
    [Required]
    public string Total { get; set; }
}