using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Rocket.Surgery.Extensions.DependencyInjection.Internals
{
    internal class ServiceWrapper : IServiceWrapper
    {
        public ServiceWrapper(ILogger logger, IServiceCollection services = default)
        {
            Services = services ?? new ServiceCollection();
            OnBuild = new ServiceProviderObservable(logger);
        }

        public IServiceCollection Services { get; }
        public ServiceProviderObservable OnBuild { get; }

        IObservable<IServiceProvider> IServiceWrapper.OnBuild => OnBuild;
    }
}
