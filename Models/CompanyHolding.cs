using System;
using System.ComponentModel.DataAnnotations;


namespace MySQLScrapper.Models
{
    public class CompanyHolding
    {
        [Key]
        public int CompanyHoldingId { get; set; }
        public string Ticker { get; set; }
        public string StockName { get; set; }
        public int NumberOfShares { get; set; }
        public double ReportedPrice { get; set; }
        public double ReportedValue { get; set; }
        public string PortfolioDate { get; set; } 
        public string DatePulled { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        [Required]
        public DateTime UpdatedAt { get; set; }

        public int CompanyId { get; set; }
        public virtual Company Company { get; set; }

        public override string ToString()
        {
            return $"CompanyHoldingId: {CompanyHoldingId} - Ticker: {Ticker} - Stock Name: {StockName} - Number of Shares: {NumberOfShares} - Reported Price {ReportedPrice} - Date Recorded {PortfolioDate} - Date Pulled {DatePulled}";
        }
    }
}