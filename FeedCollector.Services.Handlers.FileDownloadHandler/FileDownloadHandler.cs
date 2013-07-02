using FeedCollector.Core.Models;
using FeedCollector.Core.Services;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Net;
using System.Xml;

namespace FeedCollector.Services.Handlers
{
    [Export(typeof(HandlerServiceBase))]
    public class FileDownloadHandler : HandlerServiceBase
    {
        public override void Process(IEnumerable<Content> items)
        {
            var settings = LoadConfigData();
            var folder = settings
                .FirstOrDefault(s => s.Key.Equals("folder", StringComparison.OrdinalIgnoreCase))
                .Value;

            foreach (var item in items)
            {
                var client = new WebClient();
                var filename = Path.GetFileName(item.Url.LocalPath);
                var directory = Path.Combine(folder, item.Site);
                var path = Path.Combine(directory, filename);

                if (!File.Exists(path))
                {
                    Directory.CreateDirectory(directory);
                    Console.WriteLine("Downloading {0} - {1}", item.Title, item.Url.OriginalString);
                    Console.WriteLine();
                    client.DownloadFile(item.Url, path);
                }
            }
        }

        private IDictionary<string, string> LoadConfigData()
        {
            var configFile = "FileDownloadHandler.config";

            var document = new XmlDocument();
            document.Load(configFile);

            var settings = document.SelectSingleNode("settings");

            return settings.SelectNodes("setting")
                .Cast<XmlNode>()
                .Select(s => new KeyValuePair<string, string>(s.Attributes["name"].Value, s.Attributes["value"].Value))
                .ToDictionary(s => s.Key, s => s.Value);
        }
    }
}
