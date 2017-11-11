using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.Extensions.Autofac
{
    internal class ServiceAndContainerWrapper : IServiceAndContainerWrapper, IServiceBuilderAndContainerWrapper
    {
        private readonly IAutofacBuilder _context;

        public ServiceAndContainerWrapper(IAutofacBuilder context, IServiceCollection services = default)
        {
            _context = context;
            Services = services ?? new ServiceCollection();
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
    }
}
