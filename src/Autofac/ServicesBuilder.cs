using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Builders;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;

namespace Rocket.Surgery.Extensions.Autofac
{
    /// <summary>
    /// Class ServicesBuilder.
    /// </summary>
    /// <seealso cref="Builder" />
    /// <seealso cref="Microsoft.Extensions.Configuration.IConfigurationBuilder" />
    /// TODO Edit XML Comment Template for ServicesBuilder
    public class ServicesBuilder : Builder, IServicesBuilder
    {
        private readonly IConventionScanner _scanner;
        private readonly ServiceConventionItem _core;
        private readonly ServiceConventionItem _system;
        private readonly ServiceConventionItem _application;

        /// <summary>
        /// Tag for applicaiton scoped container
        /// </summary>
        public static string ApplicationTag = "__Application__";
        /// <summary>
        /// Tag for system scoped container
        /// </summary>
        public static string SystemTag = "__System__";

        /// <summary>
        /// Initializes a new instance of the <see cref="ServicesBuilder" /> class.
        /// </summary>
        /// <param name="assemblyProvider">The assembly provider.</param>
        /// <param name="assemblyCandidateFinder">The assembly candidate finder.</param>
        /// <param name="scanner"></param>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="hostingEnvironment"></param>
        public ServicesBuilder(
            IAssemblyProvider assemblyProvider,
            IAssemblyCandidateFinder assemblyCandidateFinder,
            IConventionScanner scanner,
            IServiceCollection services,
            IConfiguration configuration,
            IHostingEnvironment hostingEnvironment)
        {
            _scanner = scanner ?? throw new ArgumentNullException(nameof(scanner));

            AssemblyProvider = assemblyProvider ?? throw new ArgumentNullException(nameof(assemblyProvider));
            AssemblyCandidateFinder = assemblyCandidateFinder ?? throw new ArgumentNullException(nameof(AssemblyCandidateFinder));
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            HostingEnvironment = hostingEnvironment ?? throw new ArgumentNullException(nameof(hostingEnvironment));
            Services = services ?? throw new ArgumentNullException(nameof(services));

            _core = new ServiceConventionItem(this);
            _application = new ServiceConventionItem(this);
            _system = new ServiceConventionItem(this);
        }

        /// <summary>
        /// Builds the root container, and returns the lifetime scopes for the application and system containers
        /// </summary>
        /// <param name="containerBuilder"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public ServicesResponse Build(ContainerBuilder containerBuilder, ILogger logger)
        {
            new ServiceConventionComposer(_scanner, logger)
                .Register(new ServiceConventionContext(this));

            _core.Collection.Apply(containerBuilder);

            containerBuilder.Populate(Services);

            var container = containerBuilder.Build();

            var system = container.BeginLifetimeScope(SystemTag, s =>
            {
                _system.Collection.Apply(s);
                CustomRegistration.Register(s, _system.Services, SystemTag);
            });

            var application = container.BeginLifetimeScope(ApplicationTag, a =>
            {
                _application.Collection.Apply(a);
                CustomRegistration.Register(a, _application.Services, ApplicationTag);
            });

            return new ServicesResponse(container, system, application);
        }

        public IServiceConventionContext Container(ContainerBuilderDelegate builder)
        {
            _core.Container(builder);
            return this;
        }

        public IServiceCollection Services { get; }
        public IConfiguration Configuration { get; }
        public IHostingEnvironment HostingEnvironment { get; }
        public IAssemblyProvider AssemblyProvider { get; }
        public IAssemblyCandidateFinder AssemblyCandidateFinder { get; }
        public IServiceConventionItem System { get; }
        public IServiceConventionItem Application { get; }
        public IServicesBuilder AddDelegate(ServiceConventionDelegate @delegate)
        {
            _scanner.AddDelegate(@delegate);
            return this;
        }
        public IServicesBuilder AddConvention(IServiceConvention @delegate)
        {
            _scanner.AddConvention(@delegate);
            return this;
        }

        /// <summary>
        /// Services
        /// </summary>
        public class ServicesResponse
        {
            /// <summary>
            ///
            /// </summary>
            /// <param name="container"></param>
            /// <param name="system"></param>
            /// <param name="application"></param>
            public ServicesResponse(IContainer container, ILifetimeScope system, ILifetimeScope application)
            {
                Container = container ?? throw new ArgumentNullException(nameof(container));
                System = system ?? throw new ArgumentNullException(nameof(system));
                Application = application ?? throw new ArgumentNullException(nameof(application));
            }

            /// <summary>
            ///
            /// </summary>
            public IContainer Container { get; }
            /// <summary>
            ///
            /// </summary>
            public ILifetimeScope System { get; }
            /// <summary>
            ///
            /// </summary>
            public ILifetimeScope Application { get; }
        }
    }
}
