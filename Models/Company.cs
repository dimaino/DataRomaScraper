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
        public string NumberOfStocks { get; set; }
        public string PortfolioValue { get; set; }
        public string HoldingURL { get; set; }
        public string DateRecorded { get; set; }
        public string DatePulled { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        [Required]
        public DateTime UpdatedAt { get; set; }
        public virtual ICollection<CompanyHolding> CompanyHoldings { get; set; }

        public override string ToString()
        {
            return $"CompanyId: {CompanyId} - Name: {Name} - Number of Stocks: {NumberOfStocks} - Portfolio Value: {PortfolioValue} - Holdings Url: {HoldingURL} - Date Recorded: {DateRecorded} - Date Pulled: {DatePulled}";
        }
    }
}