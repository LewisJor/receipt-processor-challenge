using System.Text.Json;
using SimpleReceiptProcessor.Controllers;
using SimpleReceiptProcessor.Controllers.Converters;
using SimpleReceiptProcessor.Models;

namespace SimpleReceiptProcessorTests;

public class RecieptsControllerTest
{
    public static string TestProcessRequestJson1 =
      """
      {
        "retailer": "Target",
        "purchaseDate": "2022-01-01",
        "purchaseTime": "13:01",
        "items": [
          {
            "shortDescription": "Mountain Dew 12PK",
            "price": "6.49"
          },{
            "shortDescription": "Emils Cheese Pizza",
            "price": "12.25"
          },{
            "shortDescription": "Knorr Creamy Chicken",
            "price": "1.26"
          },{
            "shortDescription": "Doritos Nacho Cheese",
            "price": "3.35"
          },{
            "shortDescription": "   Klarbrunn 12-PK 12 FL OZ  ",
            "price": "12.00"
          }
        ],
        "total": "35.35"
      }
      """;

    public static string TestProcessRequestJson2 =
        """
            {
              "retailer": "M&M Corner Market",
              "purchaseDate": "2022-03-20",
              "purchaseTime": "14:33",
              "items": [
                {
                  "shortDescription": "Gatorade",
                  "price": "2.25"
                },{
                  "shortDescription": "Gatorade",
                  "price": "2.25"
                },{
                  "shortDescription": "Gatorade",
                  "price": "2.25"
                },{
                  "shortDescription": "Gatorade",
                  "price": "2.25"
                }
              ],
              "total": "9.00"
            }
            """;
    
    public Receipt Receipt1;
    public Receipt Receipt2;

    [SetUp]
    public void Setup()
    {
        var options = new JsonSerializerOptions
        {
            Converters = { new CustomTimeSpanConverter() },
            PropertyNameCaseInsensitive = true
        };
        
        Receipt1 = JsonSerializer.Deserialize<Receipt>(TestProcessRequestJson1, options)!;
        Receipt2 = JsonSerializer.Deserialize<Receipt>(TestProcessRequestJson2, options)!;
    }

    [Test]
    public void Should_Be_28_Total_Points()
    {
        ReceiptsController.ApplyRegexToReceipt(Receipt1);
        var points = ReceiptsController.CalculatePoints(Receipt1);
        Assert.That(points, Is.EqualTo(28));
    }
    
    [Test]
    public void Should_Be_109_Total_Points()
    {
        ReceiptsController.ApplyRegexToReceipt(Receipt2);
        var points = ReceiptsController.CalculatePoints(Receipt2);
        Assert.That(points, Is.EqualTo(109));
    }
}