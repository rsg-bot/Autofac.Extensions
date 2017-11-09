using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.Extensions.Autofac
{
    internal class ServiceConventionItem : IServiceConventionItem, IServicesBuilderConventionItem
    {
        private readonly IServicesBuilder _context;

        public ServiceConventionItem(IServicesBuilder context, IServiceCollection services = default)
        {
            _context = context;
            Services = services ?? new ServiceCollection();
        }

        public IServiceConventionContext Container(ContainerBuilderDelegate builder)
        {
            Collection.Add(builder);
            return (IServiceConventionContext)_context;

        }

        public ContainerBuilderCollection Collection { get; } = new ContainerBuilderCollection();

        IServicesBuilder IServicesBuilderConventionItem.Container(ContainerBuilderDelegate builder)
        {
            Collection.Add(builder);
            return _context;
        }

        public IServiceCollection Services { get; }
    }
}
