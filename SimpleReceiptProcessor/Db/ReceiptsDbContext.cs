using Microsoft.EntityFrameworkCore;
using SimpleReceiptProcessor.Models;
using SimpleReceiptProcessor.Exceptions;
using System.Text.RegularExpressions;

namespace SimpleReceiptProcessor.Db
{
    public class ReceiptsDbContext : DbContext
    {
        public DbSet<Receipt> Receipts { get; set; }
        public DbSet<Item> Items { get; set; }

        public ReceiptsDbContext(DbContextOptions<ReceiptsDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        { 
            modelBuilder.Entity<Item>()
                .HasOne(i => i.Receipt)
                .WithMany(r => r.Items)
                .HasForeignKey(i => i.ReceiptId)
                .IsRequired();
        }

        private bool SanitizeReceipt(Receipt receipt)
        {
            var retailerMatch = Regex.Match(receipt.Retailer, @"[\w\s\-&]+");
            if (retailerMatch.Success)
            {
                receipt.Retailer = retailerMatch.Value;
            }
            else
            {
                throw new DatabaseException("Retailer name is invalid");
            }

            var totalMatch = Regex.Match(receipt.Total, @"\d+\.\d{2}");
            if (totalMatch.Success)
            {
                receipt.Total = totalMatch.Value;
            }
            else
            {
                throw new DatabaseException("Total amount is invalid");
            }

            foreach (var item in receipt.Items)
            {
                item.ReceiptId = receipt.Id;
                item.Receipt = receipt;

                var descriptionMatch = Regex.Match(item.ShortDescription, @"[\w\s\-]+");
                item.ShortDescription = descriptionMatch.Success 
                    ? descriptionMatch.Value 
                    : throw new DatabaseException("Short description is invalid");

                var priceMatch = Regex.Match(item.Price, @"\d+\.\d{2}");
                item.Price = priceMatch.Success 
                    ? priceMatch.Value 
                    : throw new DatabaseException("Price is invalid");

                Items.Add(item);
            }

            return true;
        }

        /// <summary>
        /// Add a receipt and it's items to the database
        /// </summary>
        /// <param name="receipt"></param>
        /// <returns></returns>
        /// <exception cref="DatabaseException"></exception>
        public async Task<bool> AddReceiptAsync(Receipt receipt)
        {
            try
            {
                SanitizeReceipt(receipt);

                Receipts.Add(receipt);
                await SaveChangesAsync().ConfigureAwait(false);

                foreach (var item in receipt.Items)
                {
                    item.ReceiptId = receipt.Id;
                    Items.Add(item);
                }

                await SaveChangesAsync().ConfigureAwait(false);
                return true;
            }
            catch (Exception ex)
            {
                throw new DatabaseException("An error occurred while adding the receipt to the database.", ex);
            }
        }
    }
}