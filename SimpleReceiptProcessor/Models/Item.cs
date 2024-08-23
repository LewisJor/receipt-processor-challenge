using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SimpleReceiptProcessor.Models;

/// <summary>
/// A purchased item
/// </summary>
public class Item
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// The short product description for the item
    /// </summary>
    /// <remarks>
    /// pattern "^[\\w\\s\\-]+$"
    /// example: "Mountain Dew 12PK"
    /// </remarks>
    [Required]
    public string ShortDescription { get; set; }

    /// <summary>
    /// the total price paid for this item
    /// </summary>
    /// <remarks>
    /// pattern "^\\d+\\.\\d{2}$"
    /// example: "6.49"
    /// </remarks>
    [Required]
    public string Price { get; set; }
    
    /// <summary>
    /// The foreign key referencing the associated receipt
    /// </summary>
    [ForeignKey("Receipt")]
    public Guid ReceiptId { get; set; }
    public Receipt Receipt { get; set; }
}