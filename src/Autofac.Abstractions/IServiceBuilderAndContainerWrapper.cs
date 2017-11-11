using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.Extensions.Autofac
{
    public interface IServiceBuilderAndContainerWrapper
    {
        IAutofacBuilder ConfigureContainer(ContainerBuilderDelegate builder);
        IServiceCollection Services { get; }
    }
}