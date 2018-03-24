using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Builders;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Hosting;

namespace Rocket.Surgery.Extensions.DependencyInjection
{
    public abstract class ChildServicesBuilder : Builder<IServiceConventionContext>, IServiceConventionContext
    {
        protected ChildServicesBuilder(IServicesBuilder parent) : base(parent)
        {
        }

        public IConfiguration Configuration => Parent.Configuration;

        public IHostingEnvironment Environment => Parent.Environment;

        public IAssemblyProvider AssemblyProvider => Parent.AssemblyProvider;

        public IAssemblyCandidateFinder AssemblyCandidateFinder => Parent.AssemblyCandidateFinder;

        public IServiceCollection Services => Parent.Services;

        public IServiceWrapper System => Parent.System;

        public IServiceWrapper Application => Parent.Application;
        public IObservable<IServiceProvider> OnBuild => Parent.OnBuild;
        public ILogger Logger => Parent.Logger;
    }
}
