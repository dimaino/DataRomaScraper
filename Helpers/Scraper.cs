using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MySQLScrapper.Models;

namespace DataRomaScraper.Services
{
    public class Scraper
    {
        public string BaseUrl { get; set; }
        public List<string> AllLinks { get; set; }

        public Scraper(string baseUrl)
        {
            BaseUrl = baseUrl;
        }

        public async Task<HtmlDocument> PageToScrape(string page)
        {
            var httpClient = new HttpClient();
            string html = await httpClient.GetStringAsync(page);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            return doc;
        }

        public string ExtractAllHTML(HtmlDocument doc)
        {
            return doc.DocumentNode.InnerHtml;
        }

        public List<string> ExtractAllLinks(HtmlDocument doc)
        {
            return doc.DocumentNode.Descendants("a").Select(link => link.InnerHtml).ToList();
        }

        public List<string> ExtractAllLinkHrefs(HtmlDocument doc)
        {
            return doc.DocumentNode.Descendants("a").Select(a => BaseUrl + a.GetAttributeValue("href", null)).Where(u => !String.IsNullOrEmpty(u)).ToList();
        }

        public List<string> ExtractAllHoldingLinks(HtmlDocument doc)
        {
            return doc.DocumentNode.Descendants("a").Select(l => BaseUrl + l.GetAttributeValue("href", null)).Where(link => link.Contains("holdings")).ToList();
        }

        public List<string> ExtractExtraPages(HtmlDocument doc)
        {
            if(doc.DocumentNode.SelectNodes("//div[@id='pages']") != null)
            {
                return doc.DocumentNode.SelectNodes("//div[@id='pages']/a")
                    .Select(l => l.InnerHtml + "," + BaseUrl + l.GetAttributeValue("href", null))
                    .Where(it => it.Any(char.IsDigit))
                    .Select(l => l.Split(',').Last())
                    .Distinct().
                    ToList();
            }
            return null;
        }

        public List<HtmlNode> ExtractTableHeader(HtmlDocument doc)
        {
            return doc.DocumentNode.SelectNodes("//table[@id='grid']/thead/tr/td").ToList();
        }

        public static List<HtmlNode> GetAllTableResults(HtmlDocument doc)
        {
            return doc.DocumentNode.SelectNodes("//table[@id='grid']/tbody/tr/td").ToList();
        }

        public static string GetCompanyName(HtmlDocument doc)
        {
            return doc.DocumentNode.SelectSingleNode("//div[@id='f_name']//text()[not(parent::span)]").InnerText;
        }

        public static string ExtractDateRecorded(HtmlDocument doc)
        {
            return doc.DocumentNode.SelectSingleNode("//p[@id='p2']//span").InnerText;
        }

        public static string ExtractDatePulled(HtmlDocument doc)
        {
            return doc.DocumentNode.SelectSingleNode("//p[@id='p2']//span[2]").InnerText;
        }

        public static string ExtractNumberOfStocks(HtmlDocument doc)
        {
            return doc.DocumentNode.SelectSingleNode("//p[@id='p2']//span[3]").InnerText;
        }

        public static string ExtractPortfolioValue(HtmlDocument doc)
        {
            String str = doc.DocumentNode.SelectSingleNode("//p[@id='p2']//span[4]").InnerText;
            return str.Substring(1, str.Length - 2);
        }

        public List<string> ExtractAllCompanyNames(HtmlDocument doc)
        {
            return doc.DocumentNode.Descendants("a")
                .Select(l => l.InnerText + "," + l.GetAttributeValue("href", null))
                .Where(link => link.Contains("holdings"))
                .Select(link => link.Split(',').First())
                .ToList();
        }

        public async Task ExtractAllUpdatedCompanies(HtmlDocument doc)
        {
            // Read in each link
            List<string> holdingLinks = ExtractAllHoldingLinks(doc);

            List<CompanyHolding> companiesHolding = new List<CompanyHolding>();

            foreach(string link in holdingLinks)
            {
                HtmlDocument newDoc = await PageToScrape(link);
                Console.WriteLine(MapCompanyData(newDoc, link));
            }
        }

        public static Company MapCompanyData(HtmlDocument doc, string currentUrl)
        {
            return new Company {
                Name = GetCompanyName(doc),
                NumberOfStocks = Int32.Parse(ExtractNumberOfStocks(doc)),
                PortfolioValue = Double.Parse(ExtractPortfolioValue(doc)),
                HoldingURL = currentUrl,
                DateRecorded = ExtractDateRecorded(doc),
                DatePulled = ExtractDatePulled(doc),
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
        }

        public static List<CompanyHolding> MapCompaniesHoldings(HtmlDocument doc)
        {
            int num = 0;
            var tableData = GetAllTableResults(doc);
            int rows = tableData.Count;
            List<CompanyHolding> holdings = new List<CompanyHolding>();
            while(num < rows)
            {
                new CompanyHolding {
                    
                };
                String stockName = tableData[1 + num].InnerText.Substring(tableData[1 + num].InnerText.IndexOf('-') + 2, tableData[1 + num].InnerText.Length - tableData[1 + num].InnerText.IndexOf('-') - 2);
                CompanyHolding newHoldings = new CompanyHolding();
                newHoldings.Ticker = tableData[1 + num].InnerText.Substring(0, tableData[1 + num].InnerText.IndexOf('-') - 1);
                newHoldings.StockName = stockName.Replace("'","");
                newHoldings.PercentOfPortfolio = Double.Parse(tableData[2 + num].InnerText);
                newHoldings.NumberOfShares = Int32.Parse(tableData[3 + num].InnerText.Replace(",", ""));
                newHoldings.RecentActivity = tableData[4 + num].InnerText;
                // newHoldings.RecentActivity = tableData[4 + num].InnerText;
                String[] Activity = tableData[4 + num].InnerText.Split(" ");
                
                if(Activity[0].Equals(" ") || Activity[0].Equals(""))
                {
                    newHoldings.RecentActivity = "Same";
                    newHoldings.ChangePercentage = 0;
                }
                else if(Activity[0].Equals("Buy"))
                {
                    newHoldings.RecentActivity = "Buy";
                    newHoldings.ChangePercentage = 0;
                }
                else
                {
                    newHoldings.RecentActivity = Activity[0];
                    newHoldings.ChangePercentage = Double.Parse(Activity[1].TrimEnd('%'));
                }
                newHoldings.ReportedPrice = Double.Parse(tableData[5 + num].InnerText.Substring(1, tableData[5 + num].InnerText.Length - 1));
                newHoldings.Value = Double.Parse(tableData[6 + num].InnerText.Substring(1, tableData[6 + num].InnerText.Length - 1));
                newHoldings.DateRecorded = "";
                newHoldings.DatePulled = "";
                num = num + 7;
                holdings.Add(newHoldings);
            }
            return holdings;
        }
    }
}