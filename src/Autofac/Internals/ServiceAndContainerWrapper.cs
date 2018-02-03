using System;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Extensions.DependencyInjection;

namespace Rocket.Surgery.Extensions.Autofac.Internals
{
    internal class ServiceAndContainerWrapper : IAutofacContextWrapper, IServiceWrapper
    {
        private readonly IAutofacBuilder _context;

        public ServiceAndContainerWrapper(IAutofacBuilder context, IServiceCollection services = default)
        {
            _context = context;
            Services = services ?? new ServiceCollection();
            LifetimeScopeOnBuild = new LifetimeScopeObservable(context.Logger);
            ServiceProviderOnBuild = new ServiceProviderObservable(context.Logger);
        }

        public IAutofacConventionContext ConfigureContainer(ContainerBuilderDelegate builder)
        {
            Collection.Add(builder);
            return (IAutofacConventionContext)_context;

        }

        public ContainerBuilderCollection Collection { get; } = new ContainerBuilderCollection();

        void IAutofacContextWrapper.ConfigureContainer(ContainerBuilderDelegate builder)
        {
            Collection.Add(builder);
        }

        public IServiceCollection Services { get; }
        public LifetimeScopeObservable LifetimeScopeOnBuild { get; }
        public ServiceProviderObservable ServiceProviderOnBuild { get; }
        
        IObservable<ILifetimeScope> IAutofacContextWrapper.OnBuild => LifetimeScopeOnBuild;
        IObservable<IServiceProvider> IServiceWrapper.OnBuild => ServiceProviderOnBuild;

        IServiceCollection IServiceWrapper.Services => Services;
    }
}
