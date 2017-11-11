using Microsoft.Extensions.Configuration;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Extensions.DependencyInjection;

namespace Rocket.Surgery.Extensions.Autofac
{
    public interface IAutofacConventionContext : IConventionContext, IServiceAndContainerWrapper
    {
        IConfiguration Configuration { get; }
        IServicesEnvironment Environment { get; }
        IAssemblyProvider AssemblyProvider { get; }
        IAssemblyCandidateFinder AssemblyCandidateFinder { get; }
        IServiceAndContainerWrapper System { get; }
        IServiceAndContainerWrapper Application { get; }
    }
}
