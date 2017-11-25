using System;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Extensions.DependencyInjection;

namespace Rocket.Surgery.Extensions.Autofac.Internals
{
    internal class ServiceAndContainerWrapper : IServiceAndContainerWrapper, IServiceBuilderAndContainerWrapper, IServiceWrapper
    {
        private readonly IAutofacBuilder _context;

        public ServiceAndContainerWrapper(IAutofacBuilder context, IServiceCollection services = default)
        {
            _context = context;
            Services = services ?? new ServiceCollection();
            LifetimeScopeOnBuild = new LifetimeScopeObservable();
            ServiceProviderOnBuild = new ServiceProviderObservable();
        }

        public IAutofacConventionContext ConfigureContainer(ContainerBuilderDelegate builder)
        {
            Collection.Add(builder);
            return (IAutofacConventionContext)_context;

        }

        public ContainerBuilderCollection Collection { get; } = new ContainerBuilderCollection();

        IAutofacBuilder IServiceBuilderAndContainerWrapper.ConfigureContainer(ContainerBuilderDelegate builder)
        {
            Collection.Add(builder);
            return _context;
        }

        public IServiceCollection Services { get; }
        public LifetimeScopeObservable LifetimeScopeOnBuild { get; }
        public ServiceProviderObservable ServiceProviderOnBuild { get; }

        IObservable<ILifetimeScope> IServiceAndContainerWrapper.OnBuild => LifetimeScopeOnBuild;
        IObservable<ILifetimeScope> IServiceBuilderAndContainerWrapper.OnBuild => LifetimeScopeOnBuild;
        IObservable<IServiceProvider> IServiceWrapper.OnBuild => ServiceProviderOnBuild;

        IServiceCollection IServiceWrapper.Services => Services;
    }
}
