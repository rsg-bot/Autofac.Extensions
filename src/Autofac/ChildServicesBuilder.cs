using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Builders;
using Rocket.Surgery.Conventions.Reflection;

namespace Rocket.Surgery.Extensions.Autofac
{
    public abstract class ChildServicesBuilder : Builder<IServicesBuilder>, IServicesBuilder
    {
        protected readonly IServicesBuilder Builder;

        protected ChildServicesBuilder(IServicesBuilder parent) : base(parent)
        {
            Builder = parent;
        }

        public IConfiguration Configuration => Builder.Configuration;
        public IServicesEnvironment Environment => Builder.Environment;
        public IAssemblyProvider AssemblyProvider => Builder.AssemblyProvider;
        public IAssemblyCandidateFinder AssemblyCandidateFinder => Builder.AssemblyCandidateFinder;
        public IServicesBuilderConventionItem System => Builder.System;
        public IServicesBuilderConventionItem Application => Builder.Application;
        public IServicesBuilder AddDelegate(ServiceConventionDelegate @delegate) => Builder.AddDelegate(@delegate);
        public IServicesBuilder AddConvention(IServiceConvention convention) => Builder.AddConvention(convention);
        public IServiceConventionContext Container(ContainerBuilderDelegate builder)
        {
            return Builder.Container(builder);
        }
        public IServiceCollection Services => Builder.Services;
    }
}
