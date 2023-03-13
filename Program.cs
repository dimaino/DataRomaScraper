using System;
using System.IO;
using MySQLScrapper.Data;
using MySQLScrapper.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using HtmlAgilityPack;
using DataRomaScraper.Services;
using System.Linq;

using ScheduleRepeater;

namespace DataRomaScraper
{
    class Program
    {
        private static IServiceProvider _ServiceProvider;
        private static IConfigurationRoot Configuration { get; set; }
        private static Scraper _Scraper;
        private static DataContext _DataContext;

        public static void Main(string[] args)
        {
            RegisterServices();

            Repeater r = new Repeater();
            // r.StartDateTimeForRepeater(2022, 5, 11, 3, 0, 0);
            r.RepeatDaily();
            r.RepeatedMethod(async () => await UpdateEverything());

            DisposeOfServices();
        }

        private static async Task UpdateEverything()
        {
            _DataContext = new DataContext();
            _Scraper = new Scraper("https://www.dataroma.com");

            await UpdateCompaniesAndLinks(_DataContext, _Scraper);
            await UpdateCompanyHoldings(_DataContext, _Scraper);
        }

        private static async Task UpdateCompanyHoldings(DataContext DataContext, Scraper scraper)
        {
            List<CompanyHolding> companiesHolding = new List<CompanyHolding>();
            List<CompanyHoldingPage> links = await DataContext.CompanyHoldingPages.Include(co => co.Company).Where(comp => comp.Company.Newest == true).ToListAsync();

            foreach(var link in links)
            {
                Console.WriteLine(link);
                HtmlDocument newDoc = scraper.PageToScrape(link.link).GetAwaiter().GetResult();
                companiesHolding = Scraper.MapCompaniesHoldings(newDoc, DataContext);

                foreach(var cH in companiesHolding)
                {
                    if(!DataContext.CompanyHoldings.Any(compHold => compHold.PortfolioDate == cH.PortfolioDate && compHold.Ticker == cH.Ticker))
                    {
                        cH.CompanyId = link.CompanyId;
                        _DataContext.CompanyHoldings.Add(cH);
                    }
                }
            }
            DataContext.SaveChanges();
        }

        private static async Task UpdateCompaniesAndLinks(DataContext DataContext, Scraper scraper)
        {
            scraper = new Scraper("https://www.dataroma.com");
            HtmlDocument doc = await scraper.PageToScrape(scraper.BaseUrl + "/m/managers.php");

            List<string> holdingLinks = scraper.ExtractAllHoldingLinks(doc);
            List<Company> allCompanies = new List<Company>();
            List<CompanyHoldingPage> companyHoldingPage = new List<CompanyHoldingPage>();

            Dictionary<Company, List<string>> CompanyLinks = new Dictionary<Company, List<string>>();

            Console.WriteLine("SEARCHING: Searching for all company holding links.");

            Parallel.ForEach(holdingLinks, link =>
            {
                HtmlDocument newDoc = scraper.PageToScrape(link).GetAwaiter().GetResult();
                Company comp = Scraper.MapCompanyData(newDoc);

                List<string> links = new List<string>();

                if(scraper.ExtractExtraPages(newDoc) != null)
                {
                    foreach(var pagelinks in scraper.ExtractExtraPages(newDoc))
                    {
                        links.Add(pagelinks);
                    }
                }
                else
                {
                    links.Add(link);
                }
                CompanyLinks.Add(comp, links);
            });

            Console.WriteLine("CHECKING: Companies and Links.");

            foreach(var compLink in CompanyLinks)
            {
                var company = compLink.Key;
                var links = compLink.Value;

                if(!DataContext.Companys.Any(comp => comp.Name == company.Name && comp.DateRecorded == company.DateRecorded))
                {
                    company.Newest = true;
                    List<Company> companies = await DataContext.Companys.Where(com => com.Name == company.Name).ToListAsync();

                    foreach(var c in companies) 
                    { 
                        c.Newest = false; 
                    }

                    DataContext.Companys.Add(company);
                    DataContext.SaveChanges();
                    Console.WriteLine($"ADDING: {company}");
                    foreach(var link in links)
                    {
                        Console.WriteLine($"ADDING: {link}");
                        DataContext.CompanyHoldingPages.Add(new CompanyHoldingPage {
                            link = link,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now,
                            CompanyId = DataContext.Companys.Where(comp => comp.Name == company.Name && comp.Newest == true).FirstOrDefault().CompanyId
                        });
                    }
                    DataContext.SaveChanges();
                }
                else
                {
                    Console.WriteLine($"IN-DATABASE: {company}");
                    foreach(var link in links)
                    {
                        if(!DataContext.CompanyHoldingPages.Any(chp => chp.link == link))
                        {
                            Console.WriteLine($"ADDING: {link} added to database.");
                            DataContext.CompanyHoldingPages.Add(new CompanyHoldingPage {
                                link = link,
                                CreatedAt = DateTime.Now,
                                UpdatedAt = DateTime.Now,
                                CompanyId = company.CompanyId
                            });
                        }
                        else
                        {
                            Console.WriteLine($"IN-DATABASE: {link} already in database.");
                        }
                    }
                    DataContext.SaveChanges();
                }
            }
        }

        private static void RegisterServices()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            Configuration = builder.Build();

            ServiceCollection collection = new ServiceCollection();
            collection.AddDbContext<DataContext>(options => options.UseMySql(
                Configuration["ConnectionStrings:Default"], 
                new MySqlServerVersion(new Version(8, 0, 23))
            ), ServiceLifetime.Transient);
            
            _ServiceProvider = collection.BuildServiceProvider();
        }

        private static void DisposeOfServices()
        {
            if(_ServiceProvider == null)
            {
                return;
            }
            if(_ServiceProvider is IDisposable)
            {
                ((IDisposable) _ServiceProvider).Dispose();
            }
        }
    }
}
