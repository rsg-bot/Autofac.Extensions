using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.Extensions.Autofac
{
    public interface IServiceConventionItem
    {
        IServiceConventionContext Container(ContainerBuilderDelegate builder);
        IServiceCollection Services { get; }
    }
}
