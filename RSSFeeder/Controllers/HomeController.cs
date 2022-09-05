using Microsoft.AspNetCore.Mvc;
using RSSFeeder.Models;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using System.ServiceModel.Syndication;
using System.Xml;
using System.Linq;

namespace RSSFeeder.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(int index = 1, string command = "")
        {
            var rssList = ListConverter.GetList(HttpContext.Session.GetString("RSSuri"));
            SyndicationFeed rss;
            if (rssList.Count > 0)
            {
                using XmlReader reader = XmlReader.Create(rssList[0].AbsoluteUri);
                rss = SyndicationFeed.Load(reader);
            }
            else
            {
                using XmlReader reader = XmlReader.Create(@"https://habr.com/rss/interesting/");
                rss = SyndicationFeed.Load(reader);
            }

            for (int i = 1; i < rssList.Count; i++)
            {
                using XmlReader reader = XmlReader.Create(rssList[i].AbsoluteUri);
                rss = new SyndicationFeed(rss.Items.Union(SyndicationFeed.Load(reader).Items));
            }

            rss.Items = rss.Items.OrderBy(x => x.PublishDate).Reverse();

            ViewData["Itam"] = rss.Items;
            ViewData["Last"] = Math.Ceiling(rss.Items.Count() / 5.0);
            switch (command)
            {
                case "Next":
                    return Redirect($"~/Home/?index={++index}");                
                case "Past":
                    if(index > 1)                    
                        return Redirect($"~/Home/?index={--index}"); 
                    else
                        return Redirect($"~/Home/?index={index}");
                case "First":
                    return Redirect($"~/Home/?index={1}");
                case "Last":
                    return Redirect($"~/Home/?index={ViewData["Last"]}");
                default:
                    break;
            }            
            foreach (var item in rss.Items)
            {
                Console.WriteLine(item.PublishDate + ":  " + item.Title.Text);
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        private bool IsValidFeedUrl(string url)
        {
            bool isValid = true;
            try
            {
                using XmlReader reader = XmlReader.Create(url);
                Rss20FeedFormatter formatter = new Rss20FeedFormatter();
                formatter.ReadFrom(reader);
                reader.Close();
            }
            catch
            {
                isValid = false;
            }

            return isValid;
        }

        [HttpPost]
        public IActionResult AddRss(Uri RSSurl)
        {         
            if (RSSurl != null && IsValidFeedUrl(RSSurl.AbsoluteUri))
            {
                var list = ListConverter.GetList(HttpContext.Session.GetString("RSSuri"));
                if (!list.Contains(RSSurl))
                {
                    list.Add(RSSurl);
                    HttpContext.Session.SetString("RSSuri", ListConverter.GetJSON(list));
                }
            }
            return Redirect($"~/Home/?index={1}");
        }

        public IActionResult RemoveRss(int RSSintdex)
        {
            var list = ListConverter.GetList(HttpContext.Session.GetString("RSSuri"));
            list.RemoveAt(RSSintdex);
            HttpContext.Session.SetString("RSSuri", ListConverter.GetJSON(list));
            return Redirect($"~/Home/?index={1}");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}