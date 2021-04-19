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
        public string PercentOfPortfolio { get; set; }
        public string NumberOfShares { get; set; }
        public string RecentActivity { get; set; }
        public string ReportedPrice { get; set; }
        public string Value { get; set; }
        public string DateRecorded { get; set; }
        public string DatePulled { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        [Required]
        public DateTime UpdatedAt { get; set; }

        public int CompanyId { get; set; }
        public virtual Company Company { get; set; }

        public override string ToString()
        {
            return $"CompanyHoldingId: {CompanyHoldingId} - Ticker: {Ticker} - Stock Name: {StockName} - Percent of Portfolio: {PercentOfPortfolio} - Number of Shares: {NumberOfShares} - Recent Activity {RecentActivity} - Reported Price {ReportedPrice} - Value {Value} - Date Recorded {DateRecorded} - Date Pulled {DatePulled}";
        }
    }
}