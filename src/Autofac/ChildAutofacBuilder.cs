using System;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Builders;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Extensions.DependencyInjection;

namespace Rocket.Surgery.Extensions.Autofac
{
    public abstract class ChildAutofacBuilder : Builder<AutofacBuilder>, IAutofacConventionContext, IServiceConventionContext
    {

        protected ChildAutofacBuilder(AutofacBuilder parent) : base(parent, ((IBuilder)parent).Properties) { }

        public IConfiguration Configuration => Parent.Configuration;
        public IHostEnvironment Environment => Parent.Environment;
        public IAssemblyProvider AssemblyProvider => Parent.AssemblyProvider;
        public IAssemblyCandidateFinder AssemblyCandidateFinder => Parent.AssemblyCandidateFinder;

        public void ConfigureContainer(ContainerBuilderDelegate builder)
        {
            Parent.ConfigureContainer(builder);
        }
        public IServiceCollection Services => Parent.Services;
        public ILogger Logger => Parent.Logger;

        public IObservable<IServiceProvider> OnBuild => Parent.OnBuild;
        public IObservable<IContainer> OnContainerBuild => Parent.OnContainerBuild;
    }
}
