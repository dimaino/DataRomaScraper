using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;

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
            // return Int32.Parse(doc.DocumentNode.SelectSingleNode("//p[@id='p2']//span[3]").InnerText);
        }

        public static string ExtractPortfolioValue(HtmlDocument doc)
        {
            return doc.DocumentNode.SelectSingleNode("//p[@id='p2']//span[4]").InnerText;
        }

        public List<string> ExtractAllCompanyNames(HtmlDocument doc)
        {
            return doc.DocumentNode.Descendants("a")
                .Select(l => l.InnerText + "," + l.GetAttributeValue("href", null))
                .Where(link => link.Contains("holdings"))
                .Select(link => link.Split(',').First())
                .ToList();
        }
    }
}