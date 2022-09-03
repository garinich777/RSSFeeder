using Microsoft.AspNetCore.Mvc;
using RSSFeeder.Models;
using System.Diagnostics;
using System.ServiceModel.Syndication;
using System.Xml;

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
                    return Redirect($"~/Home/?index={10}");
                default:
                    break;
            }
            XmlReader reader = XmlReader.Create(@"https://habr.com/rss/interesting/");
            SyndicationFeed rss = SyndicationFeed.Load(reader);

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}