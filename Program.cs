using System;
using System.Threading.Tasks;
using DataRomaScraper.Services;
using HtmlAgilityPack;

namespace DataRomaScraper
{
    class Program
    {
        private static Scraper _Scraper;

        public static async Task Main(string[] args)
        {
            _Scraper = new Scraper("https://www.dataroma.com");
            HtmlDocument doc = await _Scraper.PageToScrape(_Scraper.BaseUrl + "/m/managers.php");

            Console.WriteLine(_Scraper.ExtractAllHTML(doc));
        }
    }
}
