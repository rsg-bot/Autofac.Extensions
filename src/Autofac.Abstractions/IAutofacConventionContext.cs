using System;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Extensions.DependencyInjection;

namespace Rocket.Surgery.Extensions.Autofac
{
    public interface IAutofacConventionContext : IConventionContext
    {
        IConfiguration Configuration { get; }
        IAssemblyProvider AssemblyProvider { get; }
        IAssemblyCandidateFinder AssemblyCandidateFinder { get; }
        void ConfigureContainer(ContainerBuilderDelegate builder);
        IServiceCollection Services { get; }
        IObservable<IContainer> OnContainerBuild { get; }
    }
}
