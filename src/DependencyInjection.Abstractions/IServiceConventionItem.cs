using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.Extensions.Autofac
{
    public interface IServiceConventionItem
    {
        IServiceCollection Services { get; }
    }
}
