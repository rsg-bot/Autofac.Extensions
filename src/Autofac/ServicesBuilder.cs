﻿using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Builders;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;

namespace Rocket.Surgery.Extensions.Autofac
{
    public class ServicesBuilder : Builder, IServicesBuilder, IServiceConventionContext
    {
        private readonly IConventionScanner _scanner;
        private readonly ServiceConventionItem _core;
        private readonly ServiceConventionItem _system;
        private readonly ServiceConventionItem _application;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationServicesBuilder" /> class.
        /// </summary>
        /// <param name="assemblyProvider">The assembly provider.</param>
        /// <param name="assemblyCandidateFinder">The assembly candidate finder.</param>
        /// <param name="scanner"></param>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="environment"></param>
        public ServicesBuilder(
            IAssemblyProvider assemblyProvider,
            IAssemblyCandidateFinder assemblyCandidateFinder,
            IConventionScanner scanner,
            IServiceCollection services,
            IConfiguration configuration,
            IServicesEnvironment environment)
        {
            _scanner = scanner ?? throw new ArgumentNullException(nameof(scanner));

            AssemblyProvider = assemblyProvider ?? throw new ArgumentNullException(nameof(assemblyProvider));
            AssemblyCandidateFinder = assemblyCandidateFinder ?? throw new ArgumentNullException(nameof(AssemblyCandidateFinder));
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            Environment = environment ?? throw new ArgumentNullException(nameof(environment));

            _core = new ServiceConventionItem(this, services);
            _application = new ServiceConventionItem(this);
            _system = new ServiceConventionItem(this);
        }

        /// <summary>
        /// Builds the root container, and returns the lifetime scopes for the application and system containers
        /// </summary>
        /// <param name="containerBuilder"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public IContainer Build(ContainerBuilder containerBuilder, ILogger logger)
        {
            Composer.Register<IServiceConventionContext, IServiceConvention, ServiceConventionDelegate>(_scanner, logger, this);

            _core.Collection.Apply(containerBuilder);
            containerBuilder.Populate(Services);

            _application.Collection.Apply(containerBuilder);
            containerBuilder.Populate(_application.Services);

            return containerBuilder.Build();
        }

        public IServiceConventionContext Container(ContainerBuilderDelegate builder)
        {
            _core.Container(builder);
            return this;
        }

        public IServiceCollection Services => _core.Services;
        public IConfiguration Configuration { get; }
        public IServicesEnvironment Environment { get; }
        public IAssemblyProvider AssemblyProvider { get; }
        public IAssemblyCandidateFinder AssemblyCandidateFinder { get; }

        IServiceConventionItem IServiceConventionContext.System => _system;
        public IServicesBuilderConventionItem System => _system;

        IServiceConventionItem IServiceConventionContext.Application => _application;
        public IServicesBuilderConventionItem Application => _application;

        public IServicesBuilder AddDelegate(ServiceConventionDelegate @delegate)
        {
            _scanner.AddDelegate(@delegate);
            return this;
        }
        IServiceConventionContext IServiceConventionContext.AddDelegate(ServiceConventionDelegate @delegate) => (IServiceConventionContext)AddDelegate(@delegate);

        public IServicesBuilder AddConvention(IServiceConvention convention)
        {
            _scanner.AddConvention(convention);
            return this;
        }
        IServiceConventionContext IServiceConventionContext.AddConvention(IServiceConvention convention) => (IServiceConventionContext)AddConvention(convention);
    }
}
