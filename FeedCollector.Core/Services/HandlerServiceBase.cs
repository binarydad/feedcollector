using FeedCollector.Core.Models;
using System.Collections.Generic;

namespace FeedCollector.Core.Services
{
    public abstract class HandlerServiceBase
    {
        public abstract void Process(IEnumerable<Content> items);
    }
}
