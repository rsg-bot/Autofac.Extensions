using System;
using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.Extensions.DependencyInjection.Internals
{
    internal class ServiceWrapper : IServiceWrapper
    {
        public ServiceWrapper(IServiceCollection services = default)
        {
            Services = services ?? new ServiceCollection();
            OnBuild = new ServiceProviderObservable();
        }

        public IServiceCollection Services { get; }
        public ServiceProviderObservable OnBuild { get; }

        IObservable<IServiceProvider> IServiceWrapper.OnBuild => OnBuild;
    }
}
