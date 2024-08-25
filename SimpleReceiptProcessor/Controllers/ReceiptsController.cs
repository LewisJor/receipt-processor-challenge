using Microsoft.AspNetCore.Mvc;
using SimpleReceiptProcessor.Models;
using System.Text.RegularExpressions;

namespace SimpleReceiptProcessor.Controllers;

/// <summary>
/// API for processing and retrieving points for receipts.
/// </summary>
[ApiController]
[Route("receipts")]
public partial class ReceiptsController : ControllerBase
{
private readonly ILogger<ReceiptsController> _logger;

    public ReceiptsController(ILogger<ReceiptsController> logger)
    {
        _logger = logger;
    }
    //^[\\w\\s\\-&]+$
    [GeneratedRegex(@"[^\w\s\-&]")]
    private static partial Regex _retailerRegex();

    // ^\\d+\\.\\d{2}$
    [GeneratedRegex(@"^\d+\\.\d{2}$")]
    private static partial Regex _moneyRegex();
    //^[\\w\\s\\-]+$
    [GeneratedRegex(@"[^\w\s\-]")]
    private static partial Regex _descriptionRegex();

    /// <summary>
    /// Submits a receipt for processing and returns a unique ID.
    /// </summary>
    /// <param name="receipt">The receipt to be processed.</param>
    /// <returns>A JSON object containing the ID of the processed receipt.</returns>
    /// <response code="200">Returns the ID of the processed receipt.</response>
    /// <response code="500">If an error occurred while processing the receipt.</response>
    [HttpPost("process")]
    public IActionResult ProcessReceipt([FromBody] Receipt receipt)
    {
        try
        {
            ApplyRegexToReceipt(receipt);
            
            var receiptId = Guid.NewGuid().ToString();
            
            InMemoryStore.Receipts[receiptId] = receipt;

            _logger.LogInformation("Receipt processed successfully with ID: {ReceiptId}", receiptId);

            return Ok(new { Id = receiptId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing the receipt.");
            return StatusCode(500, "An error occurred while processing the receipt.");
        }
    }
    
    /// <summary>
    /// Retrieves the points awarded for a processed receipt.
    /// </summary>
    /// <param name="id">The unique ID of the receipt.</param>
    /// <returns>A JSON object containing the points awarded for the receipt.</returns>
    /// <response code="200">Returns the points for the specified receipt ID.</response>
    /// <response code="404">If the receipt with the specified ID is not found.</response>
    /// <response code="500">If an error occurred while retrieving the points.</response>
    [HttpGet("{id}/points")]
    public IActionResult GetReceiptPoints([FromRoute] Guid id)
    {
        try
        {
            var receiptId = id.ToString();

            if (!InMemoryStore.Receipts.TryGetValue(receiptId, out var receipt))
            {
                _logger.LogWarning("Receipt with ID {ReceiptId} not found.", receiptId);
                return NotFound("No receipt found for that id.");
            }

            var points = CalculatePoints(receipt);

            _logger.LogInformation("Points calculated for receipt ID {ReceiptId}: {Points}", receiptId, points);

            return Ok(new { points });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while calculating points for receipt ID: {ReceiptId}", id);
            return StatusCode(500, "An error occurred while calculating points.");
        }
    }
    
    /// <summary>
    /// Apply Regex Patterns to a receipt to ensure correct points are calculated.
    /// </summary>
    /// <param name="receipt"></param>
    public static void ApplyRegexToReceipt(Receipt receipt)
    {
        receipt.Retailer = _retailerRegex().Replace(receipt.Retailer, string.Empty).Trim();
        receipt.Total = _moneyRegex().Replace(receipt.Total, string.Empty).Trim();

        foreach (var item in receipt.Items)
        {
            item.ShortDescription = _descriptionRegex().Replace(item.ShortDescription, string.Empty).Trim();
            item.Price = _moneyRegex().Replace(item.Price, string.Empty).Trim();
        }
    }

    /// <summary>
    /// Calculate the points for a receipt.
    /// </summary>
    public static int CalculatePoints(Receipt receipt)
    {
        var points = 0;

        points += receipt.Retailer.Count(char.IsLetterOrDigit);

        if (decimal.TryParse(receipt.Total, out var totalAmount))
        {
            if (totalAmount % 1 == 0)
            {
                points += 50;
            }

            if (totalAmount % 0.25m == 0)
            {
                points += 25;
            }
        }

        points += (receipt.Items.Count / 2) * 5;

        foreach (var item in receipt.Items)
        {
            var trimmedLength = item.ShortDescription.Trim().Length;

            if (trimmedLength % 3 != 0) continue;

            if (decimal.TryParse(item.Price, out var itemPrice))
            {
                points += (int)Math.Ceiling(itemPrice * 0.2m);
            }
        }

        if (receipt.PurchaseDate.Day % 2 != 0)
        {
            points += 6;
        }

        if (receipt.PurchaseTime > new TimeSpan(14, 0, 0)
            && receipt.PurchaseTime < new TimeSpan(16, 0, 0))
        {
            points += 10;
        }

        return points;
    }
}

/// <summary>
/// Lightweight storage for incoming receipts
/// TODO :: Scale to a supported NOSQL DB, but not important for this solution.
/// </summary>
public static class InMemoryStore
{
    public static Dictionary<string, Receipt> Receipts = new();
}