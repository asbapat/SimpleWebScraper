using SimpleWebScrapper.Builders;
using SimpleWebScrapper.Data;
using SimpleWebScrapper.Workers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SimpleWebScrapper
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Enter the City to scrape information from");
                var city = Console.ReadLine();

                Console.WriteLine("Enter the category to scrape information");
                var category = Console.ReadLine();

                
                using (WebClient client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    var url = $"https://{city.Replace(" ", string.Empty)}.craigslist.org/search/{category}";
                    
                    string content = client.DownloadString(url);

                    ScrapeCriteria scrapeCriteria = new ScrapeCriteriaBuilder()
                                                                .WithData(content)
                                                                .WithRegex("<a href=\"(.*?)\" data-id=\"(.*?)\" class=\"result-title hdrlnk\">(.*?)</a>")
                                                                .WithRegexOptions(RegexOptions.ExplicitCapture)
                                                                .WithParts(new ScrapeCriteriaPartBuilder()
                                                                    .WithRegex(">(.*?)</a>")
                                                                    .WithRegexOptions(RegexOptions.Singleline)
                                                                    .Build())
                                                                .WithParts(new ScrapeCriteriaPartBuilder()
                                                                    .WithRegex("href=\"(.*?)\"")
                                                                    .WithRegexOptions(RegexOptions.Singleline)
                                                                    .Build())
                                                                .Build();

                    Scraper scraper = new Scraper();

                    var elements = scraper.Scrape(scrapeCriteria);
                    if (elements.Any())
                    {
                        foreach (var element in elements)
                        {
                            Console.WriteLine(element);
                        }
                    }
                    else
                    {
                        Console.WriteLine("No matches found");
                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
            
        }
    }
}
