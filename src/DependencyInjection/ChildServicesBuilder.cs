using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Builders;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Hosting;

namespace Rocket.Surgery.Extensions.DependencyInjection
{
    public abstract class ChildServicesBuilder : Builder<IServicesBuilder>, IServicesBuilder
    {
        protected readonly IServicesBuilder Builder;

        protected ChildServicesBuilder(IServicesBuilder parent) : base(parent)
        {
            Builder = parent;
        }

        public IConfiguration Configuration => Builder.Configuration;

        public IHostingEnvironment Environment => Builder.Environment;

        public IAssemblyProvider AssemblyProvider => Builder.AssemblyProvider;

        public IAssemblyCandidateFinder AssemblyCandidateFinder => Builder.AssemblyCandidateFinder;

        public IServiceCollection Services => Builder.Services;

        public IServiceWrapper System => Builder.System;

        public IServiceWrapper Application => Builder.Application;
        public IObservable<IServiceProvider> OnBuild => Builder.OnBuild;

        public IServicesBuilder AddDelegate(ServiceConventionDelegate @delegate) => Builder.AddDelegate(@delegate);

        public IServicesBuilder AddConvention(IServiceConvention convention) => Builder.AddConvention(convention);
    }
}
