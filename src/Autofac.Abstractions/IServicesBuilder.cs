using Microsoft.Extensions.Configuration;
using Rocket.Surgery.Builders;
using Rocket.Surgery.Conventions.Reflection;

namespace Rocket.Surgery.Extensions.Autofac
{
    /// <summary>
    /// Class IServicesBuilder.
    /// </summary>
    /// TODO Edit XML Comment Template for IServicesBuilder
    public interface IServicesBuilder : IBuilder, IServiceConventionItem
    {
        IConfiguration Configuration { get; }
        IServicesEnvironment Environment { get; }
        IAssemblyProvider AssemblyProvider { get; }
        IAssemblyCandidateFinder AssemblyCandidateFinder { get; }
        IServicesBuilderConventionItem System { get; }
        IServicesBuilderConventionItem Application { get; }
        IServicesBuilder AddDelegate(ServiceConventionDelegate @delegate);
        IServicesBuilder AddConvention(IServiceConvention convention);
    }
}
