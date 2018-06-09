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
    public abstract class ChildAutofacBuilder : Builder<AutofacBuilderBase>, IAutofacConventionContext
    {

        protected ChildAutofacBuilder(AutofacBuilderBase parent) : base(parent, ((IBuilder)parent).Properties) { }

        public IConfiguration Configuration => Parent.Configuration;
        public IHostingEnvironment Environment => Parent.Environment;
        public IAssemblyProvider AssemblyProvider => Parent.AssemblyProvider;
        public IAssemblyCandidateFinder AssemblyCandidateFinder => Parent.AssemblyCandidateFinder;
        public IAutofacContextWrapper System => Parent.System;
        public IAutofacContextWrapper Application => Parent.Application;

        public void ConfigureContainer(ContainerBuilderDelegate builder)
        {
            Parent.ConfigureContainer(builder);
        }
        public IServiceCollection Services => Parent.Services;
        public IObservable<ILifetimeScope> OnBuild => Parent.OnBuild;
        public ILogger Logger => Parent.Logger;
    }
}
