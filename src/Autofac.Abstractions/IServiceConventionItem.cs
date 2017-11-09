using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.Extensions.Autofac
{
    public interface IServiceConventionItem
    {
        IServiceConventionContext Container(ContainerBuilderDelegate builder);
        IServiceCollection Services { get; }
    }

    public interface IServicesBuilderConventionItem
    {
        IServicesBuilder Container(ContainerBuilderDelegate builder);
        IServiceCollection Services { get; }
    }
}
