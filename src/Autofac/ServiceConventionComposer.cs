using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Scanners;

namespace Rocket.Surgery.Extensions.Autofac
{
    public class ServiceConventionComposer : ConventionComposer<IServiceConventionContext, IServiceConvention, ServiceConventionDelegate>
    {
        public ServiceConventionComposer(IConventionScanner scanner, ILogger logger) : base(scanner, logger) { }
    }
}
