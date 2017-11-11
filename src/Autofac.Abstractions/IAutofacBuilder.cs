using Microsoft.Extensions.Configuration;
using Rocket.Surgery.Builders;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Extensions.DependencyInjection;
using Rocket.Surgery.Hosting;

namespace Rocket.Surgery.Extensions.Autofac
{
    /// <summary>
    /// Class IAutofacBuilder.
    /// </summary>
    /// TODO Edit XML Comment Template for IAutofacBuilder
    public interface IAutofacBuilder : IBuilder, IServiceBuilderAndContainerWrapper
    {
        IConfiguration Configuration { get; }
        IHostingEnvironment Environment { get; }
        IAssemblyProvider AssemblyProvider { get; }
        IAssemblyCandidateFinder AssemblyCandidateFinder { get; }
        IServiceBuilderAndContainerWrapper System { get; }
        IServiceBuilderAndContainerWrapper Application { get; }
        IAutofacBuilder AddDelegate(AutofacConventionDelegate @delegate);
        IAutofacBuilder AddConvention(IAutofacConvention convention);
    }
}
