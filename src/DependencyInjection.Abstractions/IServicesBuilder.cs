using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Builders;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Hosting;

namespace Rocket.Surgery.Extensions.DependencyInjection
{
    /// <summary>
    /// Class IServicesBuilder.
    /// </summary>
    /// TODO Edit XML Comment Template for IServicesBuilder
    public interface IServicesBuilder : IBuilder, IServiceWrapper
    {
        IConfiguration Configuration { get; }
        IHostingEnvironment Environment { get; }
        IAssemblyProvider AssemblyProvider { get; }
        IAssemblyCandidateFinder AssemblyCandidateFinder { get; }
        IServiceWrapper System { get; }
        IServiceWrapper Application { get; }
        IServicesBuilder AddDelegate(ServiceConventionDelegate @delegate);
        IServicesBuilder AddConvention(IServiceConvention convention);
    }
}
