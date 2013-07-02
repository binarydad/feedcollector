using System;

namespace FeedCollector.Core.Models
{
    public class Content
    {
        public Guid ID { get; set; }
        public string Title { get; set; }
        public string Site { get; set; }
        public DateTime? Date { get; set; }
        public Uri Url { get; set; }
    }
}
