using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using AngleSharp;
using AngleSharp.Html.Parser;
using AngleSharp.Html.Dom;
using AngleSharp.Dom;
using System.Text.RegularExpressions;


namespace WebCrawler
{
    class Program
    {
        static void Main(string[] args)
        {

            RunSearch();
        }

        static string GetQuery()
        {
            string q;
            do
            {
                Console.WriteLine("Please enter a term you wish to search: ");
                q = Console.ReadLine().Trim();
            }
            while (String.IsNullOrEmpty(q));

            Console.Clear();

            return q;
        }

        static void RunSearch()
        {
            bool run = true;
            while (run)
            {

                string query = GetQuery();
                Crawler.StartCrawler(query);
                Console.WriteLine();
                Console.WriteLine();

                Console.WriteLine("Do you want to search again? Y/N");
                string response = Console.ReadLine().Trim().ToUpper();
                if(response == "N")
                {
                    run = !run;
                }
                else
                {
                    Console.Clear();
                }


            }
        }
       
    }
}
