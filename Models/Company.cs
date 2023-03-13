using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace MySQLScrapper.Models
{
    public class Company
    {
        [Key]
        public int CompanyId { get; set; }
        public string Name { get; set; }
        public int NumberOfStocks { get; set; }
        public double PortfolioValue { get; set; }
        public string DateRecorded { get; set; }
        public string DatePulled { get; set; }
        public bool Newest { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        [Required]
        public DateTime UpdatedAt { get; set; }
        public virtual ICollection<CompanyHolding> CompanyHoldings { get; set; }
        public virtual ICollection<CompanyHoldingPage> CompanyHoldingPages { get; set; }

        public override string ToString()
        {
            // return $"Name: {Name} - Number of Stocks: {NumberOfStocks}";
            return $"CompanyId: {CompanyId} - Name: {Name} - Number of Stocks: {NumberOfStocks} - Portfolio Value: {PortfolioValue} - Date Recorded: {DateRecorded} - Date Pulled: {DatePulled}";
        }
    }
}