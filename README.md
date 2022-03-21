# WebCrawler

## Overview


Simple Console application for a webcrawler using C# to crawl through https://www.nature.com

Application runs in Visual Studio after building and then running

Program.cs contains runs the Crawler class and takes in input for the search term.

Article.cs creates the Article class to contain titles and links for each article returned in output.

<p align="justify">
Crawler.cs implements a crawler that starts at https://www.nature.com/news and checks if there are any articles listed with titles containing the search term.
Crawler.cs also checks the robots.txt file to keep track of urls that the crawler is not allowed to visit.
To limit the amount of urls visited, a max visit of 15 urls was implemented, as well as a tracker to keep track of the amount of urls currently visited.
Urls that contain the search term are added to a list of sites to visit, then they are visited and checked for any related articles that may also contain the search term in their title.
</p>


