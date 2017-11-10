using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Builders;
using Rocket.Surgery.Conventions.Reflection;

namespace Rocket.Surgery.Extensions.DependencyInjection
{
    /// <summary>
    /// Class IServicesBuilder.
    /// </summary>
    /// TODO Edit XML Comment Template for IServicesBuilder
    public interface IServicesBuilder : IBuilder
    {
        IConfiguration Configuration { get; }
        IServicesEnvironment Environment { get; }
        IAssemblyProvider AssemblyProvider { get; }
        IAssemblyCandidateFinder AssemblyCandidateFinder { get; }
        IServiceCollection Services { get; }
        IServiceCollection System { get; }
        IServiceCollection Application { get; }
        IServicesBuilder AddDelegate(ServiceConventionDelegate @delegate);
        IServicesBuilder AddConvention(IServiceConvention convention);
    }
}
