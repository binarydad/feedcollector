using FeedCollector.Core.Models;
using System.Collections.Generic;

namespace FeedCollector.Core.Services
{
    public abstract class CollectorServiceBase : ServiceBase
    {
        //public IStorageService Storage { get; set; }
        public IEnumerable<HandlerServiceBase> Handlers { get; set; }

        protected abstract IEnumerable<Content> Retrieve();

        public void Process()
        {
            // get videos from source
            var items = Retrieve();

            // process videos through each handler
            foreach (var h in Handlers)
            {
                h.Process(items);
            }
        }
    }
}
