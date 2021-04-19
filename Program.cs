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

namespace DataRomaScraper
{
    class Program
    {
        private static IServiceProvider _ServiceProvider;
        private static IConfigurationRoot Configuration { get; set; }
        private static Scraper _Scraper;
        private static DataContext _DataContext;

        public static async Task Main(string[] args)
        {
            RegisterServices();

            _DataContext = new DataContext();

            _Scraper = new Scraper("https://www.dataroma.com");
            HtmlDocument doc = await _Scraper.PageToScrape(_Scraper.BaseUrl + "/m/managers.php");

            List<string> holdingLinks = _Scraper.ExtractAllHoldingLinks(doc);
            List<CompanyHolding> companiesHolding = new List<CompanyHolding>();


            // int i = 1;
            foreach(string link in holdingLinks)
            {
                HtmlDocument newDoc = await _Scraper.PageToScrape(link);

                Company comp = Scraper.MapCompanyData(newDoc, link);
                comp.DatePulled = "31 Mar 2021";

                Company comp1 = _DataContext.Companys.Where(c => c.Name == comp.Name && c.DatePulled == comp.DatePulled).FirstOrDefault();
                if(comp1 == null)
                {
                    _DataContext.Companys.Add(comp);
                    _DataContext.SaveChanges();
                    if(_Scraper.ExtractExtraPages(newDoc) != null)
                    {
                        foreach(var o in _Scraper.ExtractExtraPages(newDoc))
                        {
                            HtmlDocument newDoc1 = await _Scraper.PageToScrape(o);
                            companiesHolding = Scraper.MapCompaniesHoldings(newDoc1);

                            Company addedCompany = _DataContext.Companys.Where(co => co.Name == comp.Name && co.DatePulled == comp.DatePulled).FirstOrDefault();
                            
                            foreach(var cH in companiesHolding)
                            {
                                cH.CompanyId = addedCompany.CompanyId;
                                cH.DateRecorded = addedCompany.DateRecorded;
                                cH.DatePulled = addedCompany.DatePulled;
                                cH.CreatedAt = DateTime.Now;
                                cH.UpdatedAt = DateTime.Now;
                                _DataContext.CompanyHoldings.Add(cH);
                            }
                        }
                    }
                    else
                    {
                        companiesHolding = Scraper.MapCompaniesHoldings(newDoc);

                        Company addedCompany = _DataContext.Companys.Where(co => co.Name == comp.Name && co.DatePulled == comp.DatePulled).FirstOrDefault();
                        
                        foreach(var cH in companiesHolding)
                        {
                            Console.WriteLine(cH);
                            cH.CompanyId = addedCompany.CompanyId;
                            cH.DateRecorded = addedCompany.DateRecorded;
                            cH.DatePulled = addedCompany.DatePulled;
                            cH.CreatedAt = DateTime.Now;
                            cH.UpdatedAt = DateTime.Now;
                            _DataContext.CompanyHoldings.Add(cH);
                        }
                        Console.WriteLine();
                    }
                }
            }
            _DataContext.SaveChanges();

            DisposeOfServices();
        }

        private static void RegisterServices()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            Configuration = builder.Build();

            ServiceCollection collection = new ServiceCollection();
            collection.AddDbContext<DataContext>(options => options.UseMySql(
                Configuration.GetConnectionString("DefaultConnection"), 
                new MySqlServerVersion(new Version(8, 0, 23))
            ));
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
