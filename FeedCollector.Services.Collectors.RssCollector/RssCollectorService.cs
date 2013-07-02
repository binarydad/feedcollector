using FeedCollector.Core.Models;
using FeedCollector.Core.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Xml;

namespace FeedCollector.Services.Collectors
{
    [Export(typeof(CollectorServiceBase))]
    public class RssCollectorService : CollectorServiceBase
    {
        private int _age = 1; // day
        private IEnumerable<Uri> _feeds; //
        private IEnumerable<string> _formats;

        protected override IEnumerable<Content> Retrieve()
        {
            LoadConfigData();

            return _feeds
                .ToList()
                .SelectMany(u =>
                {
                    var reader = XmlReader.Create(u.OriginalString/*, new XmlReaderSettings { IgnoreComments = true, IgnoreWhitespace = true, ConformanceLevel = ConformanceLevel.Fragment }*/);
                    var feed = SyndicationFeed.Load(reader);

                    return feed
                        .Items
                        .Select(f =>
                            {
                                if (f.Links != null && f.Links.Any())
                                {
                                    // TODO: handle mp3 mime type!
                                    var media = f.Links.FirstOrDefault(l => l.MediaType != null && this._formats.Contains(l.MediaType));

                                    if (media != null)
                                    {
                                        return new Content
                                        {
                                            Title = f.Title.Text,
                                            Url = media.Uri,
                                            Date = f.PublishDate.Date,
                                            Site = feed.Title.Text
                                        };
                                    }
                                }

                                return null;
                            })
                        .Where(c => c != null && c.Date > DateTime.Now.AddDays(this._age * -1))
                        .ToList();
                });
        }

        private void LoadConfigData()
        {
            var configFile = "RssCollectorService.config";

            var document = new XmlDocument();
            document.Load(configFile);

            var root = document.SelectSingleNode("/rss");

            this._age = Convert.ToInt32(root.SelectSingleNode("age").InnerText);
            this._feeds = root.SelectNodes("feeds/feed").Cast<XmlNode>().Select(n => new Uri(n.InnerText));
            this._formats = root.SelectNodes("formats/format").Cast<XmlNode>().Select(n => n.InnerText);
        }
    }
}
