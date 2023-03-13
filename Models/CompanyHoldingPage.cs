using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace MySQLScrapper.Models
{
    public class CompanyHoldingPage
    {
        [Key]
        public int CompanyHoldingPageId { get; set; }
        public string link { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        [Required]
        public DateTime UpdatedAt { get; set; }
        public int CompanyId { get; set; }
        public virtual Company Company { get; set; }

        public override string ToString()
        {
            return $"ID: {CompanyHoldingPageId} - Link: {link}";
        }
    }
}