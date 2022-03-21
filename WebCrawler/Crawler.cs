using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using HtmlAgilityPack;

namespace WebCrawler
{
    public static class Crawler
    {
        // Keep track of urls that are visited and need to be visited
        public static List<string> ToVisit = new List<string>();
        public static List<string> Visited = new List<string>();

        // URLS specified in Robots.txt
        public static List<string> Disallowed = new List<string>();

        // Keep track of all the Articles that are related
        public static List<Article> Articles = new List<Article>();

        // Limit the amount of urls visited
        public static int MaxVisits = 15;
        public static int currentVisits = 0;
        public static string Query { get; set; }
        public static string url = "https://www.nature.com";



        public static void StartCrawler(string query)
        {
            Query = query.ToLower();
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = new HtmlDocument();
            doc = web.Load(url + "/news");

            Console.WriteLine("Searching...");

            // Getting articles on the first page of Nature.com
            var divs = doc.DocumentNode.Descendants("div").Where(x => x.GetAttributeValue("class", "")
            .Equals("c-card__container u-flex u-flex-align-items--flex-start")).ToList();

            foreach (var div in divs)
            {
                string title = div.Descendants("h3").FirstOrDefault().InnerText.ToLower();
                if (title.Contains(Query))
                {
                    string link = div.Descendants("a").FirstOrDefault().ChildAttributes("href").FirstOrDefault().Value;
                    if (!link.Contains(url))
                    {
                        link = url + link;
                    }

                    Article article = new Article
                    {
                        Title = div.Descendants("h3").FirstOrDefault().InnerText,
                        Link = link
                    };

                    Articles.Add(article);
                    ToVisit.Add(article.Link);

                }
            }
            FindRobotsTxt();
            Crawl();
        }

        private static void PrintArticle(Article a)
        {
            Console.WriteLine("Title: " + a.Title);
            Console.WriteLine("URL: " + a.Link);
            Console.WriteLine("===========================");
            Console.WriteLine();
        }

        //Read robots.txt and keep track of urls not allowed to visit
        private static void FindRobotsTxt()
        {
            string robo = url + "/robots.txt";
            WebClient client = new WebClient();
            using (Stream data = client.OpenRead(robo))
            {
                using (StreamReader reader = new StreamReader(data))
                {
                    while (reader.Peek() >= 0)
                    {
                        string line = reader.ReadLine();
                        if (line.Contains("Disallow"))
                        {
                            var split = line.Split(' ');
                            Disallowed.Add(url + split[1]);
                        }
                    }


                }
            }
        }

        // Keep crawling through the links found
        private static void Crawl()
        {
            Console.WriteLine("Searching...");
            //If all links have been found or max amount of visits have been reached, print out results and then reset lists
            if(ToVisit.Count() < 1 || currentVisits == MaxVisits)
            {
                Console.Clear();
                if(Articles.Count() < 1)
                {
                    Console.WriteLine($"Sorry, no articles were found relating to the term {Query} :(");
                }
                else
                {
                    Console.WriteLine($"There are {Articles.Count()} Articles relating to the term: {Query}");
                    Console.WriteLine();
                    foreach (var article in Articles)
                    {
                        PrintArticle(article);
                    }

                    Disallowed.Clear();
                    ToVisit.Clear();
                    Articles.Clear();
                    Visited.Clear();
                    currentVisits = 0;

                }
                return;
            }

            var visit = ToVisit[0];
            ToVisit.RemoveAt(0);
            if (Visited.Contains(visit) || Disallowed.Contains(visit))
            {
                Crawl();
            }
            else
            {
                VisitPage(visit);
                Crawl();
            }

        }

        private static void VisitPage(string visit)
        {
            Visited.Add(visit);
            currentVisits += 1;
            //Console.WriteLine(visit);
            
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = new HtmlDocument();
            try { doc = web.Load(visit);  }
            catch (Exception e)
            {
                currentVisits -= 1;
                return;
            }

            var related = doc.DocumentNode.SelectNodes("//ul[@class='c-article-related-articles u-clearfix u-hide-print']/li[@class='c-article-related-articles__item u-clearfix']");

            if(related != null)
            {
                foreach (var a in related)
                {
                    string title = a.Descendants("h3").FirstOrDefault().InnerText.ToLower().Replace("\n", "").TrimStart();
                    if (title.Contains(Query))
                    {

                        Article article = new Article
                        {
                            Title = a.Descendants("h3").FirstOrDefault().InnerText.Replace("\n", "").TrimStart(),
                            Link = a.Descendants("a").FirstOrDefault().ChildAttributes("href").FirstOrDefault().Value
                        };

                        Articles.Add(article);
                        ToVisit.Add(article.Link);

                    }
                }
            }     
        }




    }
}
