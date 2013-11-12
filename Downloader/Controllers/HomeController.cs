using Downloader.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Downloader.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDownloadService downloadService;

        public HomeController(IDownloadService downloadService)
        {
            this.downloadService = downloadService;
        }

        public HomeController() : this(new YouTubeService())
        {
        }

        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Download/SourceUrl

        public ActionResult Download(string sourceUrl)
        {
            var downloadLinks = downloadService.FetchDownloadDetails(sourceUrl);

            return PartialView("_Download", downloadLinks);
        }
    }
}
