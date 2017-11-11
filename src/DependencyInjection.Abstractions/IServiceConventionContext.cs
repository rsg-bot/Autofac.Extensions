using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;

namespace Rocket.Surgery.Extensions.DependencyInjection
{
    public interface IServiceConventionContext : IConventionContext
    {
        IConfiguration Configuration { get; }
        IServicesEnvironment Environment { get; }
        IAssemblyProvider AssemblyProvider { get; }
        IAssemblyCandidateFinder AssemblyCandidateFinder { get; }
        IServiceCollection Services { get; }
        IServiceCollection System { get; }
        IServiceCollection Application { get; }
    }
}
