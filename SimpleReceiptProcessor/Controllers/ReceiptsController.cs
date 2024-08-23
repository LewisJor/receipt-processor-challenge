using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleReceiptProcessor.Controllers.Dtos;
using SimpleReceiptProcessor.Db;
using SimpleReceiptProcessor.Exceptions;
using SimpleReceiptProcessor.Models;

namespace SimpleReceiptProcessor.Controllers;

[ApiController]
[Route("receipts")]
public class ReceiptsController : ControllerBase
{
    private readonly ReceiptsDbContext _dbContext;

    public ReceiptsController(ReceiptsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Process a receipt and store it in the database
    /// </summary>
    /// <param name="receipt"></param>
    /// <returns>Receipt Id</returns>
    [HttpPost("process")]
    public async Task<IActionResult> ProcessReceiptAsync([FromBody] ReceiptDto receiptDto)
    {
        if (receiptDto.Items.Count == 0)
        {
            return BadRequest("The receipt must have at least one item.");
        }

        var receipt = new Receipt
        {
            Retailer = receiptDto.Retailer,
            PurchaseDate = receiptDto.PurchaseDate,
            PurchaseTime = receiptDto.PurchaseTime,
            Total = receiptDto.Total,
        };
        
        foreach (var itemDto in receiptDto.Items)
        {
            receipt.Items.Add(new Item
            {
                ShortDescription = itemDto.ShortDescription,
                Price = itemDto.Price,
                Receipt = receipt,
                ReceiptId = receipt.Id
            });
        }

        try
        {
            await _dbContext.AddReceiptAsync(receipt);
        }
        catch (DatabaseException e)
        {
            return BadRequest(e.Message);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }

        return Ok(new { id = receipt.Id.ToString() });
    }

    /// <summary>
    /// Get the points awarded for a specific receipt by its ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Points awarded</returns>
    [HttpGet("{id}/points")]
    public async Task<IActionResult> GetReceiptPointsAsync([FromRoute] Guid id)
    {
        var receipt = await _dbContext.Receipts
            .Include(r => r.Items)
            .FirstOrDefaultAsync(r => r.Id == id)
            .ConfigureAwait(false);

        if (receipt == null)
        {
            return NotFound("No receipt found for that id.");
        }

        var points = CalculatePoints(receipt);

        return Ok(new { points });
    }

    /// <summary>
    /// Calculate the points for a receipt.
    /// </summary>
    /// <remarks>
    /// One point for every alphanumeric character in the retailer name.
    /// 50 points if the total is a round dollar amount with no cents.
    /// 25 points if the total is a multiple of 0.25.
    /// 5 points for every two items on the receipt.
    /// If the trimmed length of the item description is a multiple of 3, multiply the price by 0.2 and round up to the nearest integer. The result is the number of points earned.
    /// 6 points if the day in the purchase date is odd.
    /// 10 points if the time of purchase is after 2:00pm and before 4:00pm.
    /// </remarks>
    /// <param name="receipt"></param>
    /// <returns></returns>
    private static int CalculatePoints(Receipt receipt)
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