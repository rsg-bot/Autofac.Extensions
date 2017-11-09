using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.Extensions.Autofac
{
    internal class ServiceConventionItem : IServiceConventionItem
    {
        private readonly IServiceConventionContext _context;

        public ServiceConventionItem(IServiceConventionContext context)
        {
            _context = context;
        }

        public IServiceConventionContext Container(ContainerBuilderDelegate builder)
        {
            Collection.Add(builder);
            return _context;

        }

        public ContainerBuilderCollection Collection { get; } = new ContainerBuilderCollection();

        public IServiceCollection Services { get; } = new ServiceCollection();
    }
}