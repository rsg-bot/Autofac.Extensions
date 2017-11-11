using Microsoft.Extensions.Configuration;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Extensions.DependencyInjection;
using Rocket.Surgery.Hosting;

namespace Rocket.Surgery.Extensions.Autofac
{
    public interface IAutofacConventionContext : IConventionContext, IServiceAndContainerWrapper
    {
        IConfiguration Configuration { get; }
        IHostingEnvironment Environment { get; }
        IAssemblyProvider AssemblyProvider { get; }
        IAssemblyCandidateFinder AssemblyCandidateFinder { get; }
        IServiceAndContainerWrapper System { get; }
        IServiceAndContainerWrapper Application { get; }
    }
}
