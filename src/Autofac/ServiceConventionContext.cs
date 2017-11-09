using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;

namespace Rocket.Surgery.Extensions.Autofac
{
    public class ServiceConventionContext : ConventionContext, IServiceConventionContext
    {
        private readonly IServicesBuilder _builder;

        public ServiceConventionContext(IServicesBuilder builder)
        {
            _builder = builder ?? throw new ArgumentNullException(nameof(builder));
        }

        public IConfiguration Configuration => _builder.Configuration;

        public IHostingEnvironment HostingEnvironment => _builder.HostingEnvironment;

        public IAssemblyProvider AssemblyProvider => _builder.AssemblyProvider;

        public IAssemblyCandidateFinder AssemblyCandidateFinder => _builder.AssemblyCandidateFinder;

        public IServiceConventionContext Container(ContainerBuilderDelegate builder) => _builder.Container(builder);

        public IServiceConventionItem System => _builder.System;

        public IServiceConventionItem Application => _builder.Application;

        IServiceCollection IServiceConventionItem.Services => _builder.Services;
    }
}
