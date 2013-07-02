using FeedCollector.Core.Services;
using System.ComponentModel.Composition.Hosting;
using System.Linq;

namespace FeedCollector.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = GetContainer();

            var collector = container.GetExport<CollectorServiceBase>().Value;
            var handlers = container.GetExports<HandlerServiceBase>().Select(h => h.Value).ToList(); 

            collector.Handlers = handlers;
            collector.Process();
        }

        static CompositionContainer GetContainer()
        {
            var catalog = new DirectoryCatalog(@".\", "*.dll");
            return new CompositionContainer(catalog);
        }
    }
}
