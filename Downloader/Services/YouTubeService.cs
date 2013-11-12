using Downloader.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;

namespace Downloader.Services
{
    public class YouTubeService : IDownloadService
    {
        private WebClient client;

        public YouTubeService()
        {
            client = new WebClient();
            client.Headers[HttpRequestHeader.Cookie] = "SID=ABCDEFGABCDEFGABCDEFGABCDEFGABCDEFGABCDEFGABCDEFGABCDEFGABCDEFGABCDEFGABCDEFGABCDEFGABCDEFGABCDEFGABCDEFGABCDEFG";
        }

        public ICollection<DownloadDetail> FetchDownloadDetails(string sourceUrl)
        {
            var result = new List<DownloadDetail>();
            var page = client.DownloadString(sourceUrl);
            var title = WebUtility.HtmlDecode(ParseTitle(page));
            var streams = ParseStreams(page);

            foreach (var stream in streams)
            {
                result.Add(new DownloadDetail
                {
                    Url = stream.Url + "&signature=" + stream.Sig,
                    Name = String.Format("{0} - {1}{2}", title, stream.Quality, MimeTypes.GetExtension(stream.ContentType))
                });
            }
            
            return result;
        }

        private ICollection<YouTubeStream> ParseStreams(string html)
        {
            var result = new List<YouTubeStream>();

            var matches = Regex.Matches(html, "url_encoded_fmt_stream_map\": \"([^\"]*)?\"");
            foreach (Match match in matches)
            {
                var streams = match.Groups[1].Value.Split(',');
                foreach (var stream in streams)
                {
                    var parameters = HttpUtility.ParseQueryString(stream.Replace("\\u0026", "&"));
                    var contentType = parameters["type"].Split(';')[0];
                    var codecs = parameters["type"].Split('"');

                    result.Add(new YouTubeStream
                    {
                        ITag = Int32.Parse(parameters["itag"]),
                        Sig = parameters["sig"],
                        FallbackHost = parameters["fallback_host"],
                        Url = parameters["url"],
                        ContentType = parameters["type"].Split(';')[0],
                        Codecs = codecs.Count() > 1 ? codecs[1] : "",
                        Quality = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(parameters["quality"])
                    });
                }
            }

            return result;
        }

        private string ParseTitle(string html)
        {
            return Regex.Match(html, @"<meta name=""title"" content=""(.+?)"">").Groups[1].Value;
        }
    }

    public class YouTubeStream
    {
        public int ITag { get; set; }
        public string Sig { get; set; }
        public string FallbackHost { get; set; }
        public string Url { get; set; }
        public string Quality { get; set; }
        public string ContentType { get; set; }
        public string Codecs { get; set; }

        public string Type
        {
            get
            {
                return String.IsNullOrWhiteSpace(Codecs) ? ContentType : ContentType + "; codecs=\"" + Codecs + "\"";
            }
        }
    }
}