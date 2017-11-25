using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Builders;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Extensions.DependencyInjection.Internals;
using Rocket.Surgery.Hosting;

namespace Rocket.Surgery.Extensions.DependencyInjection
{
    /// <summary>
    /// Class ApplicationServicesBuilder.
    /// </summary>
    /// <seealso cref="Builder" />
    /// <seealso cref="Microsoft.Extensions.Configuration.IConfigurationBuilder" />
    /// TODO Edit XML Comment Template for ApplicationServicesBuilder
    public class ApplicationServicesBuilder : Builder, IServicesBuilder, IServiceConventionContext
    {
        private readonly IConventionScanner _scanner;
        private readonly ServiceProviderObservable _onBuild;
        private readonly ServiceWrapper _application;
        private readonly ServiceWrapper _system;

        /// <summary>
        /// Tag for applicaiton scoped container
        /// </summary>
        public static string ApplicationTag = "__Application__";
        /// <summary>
        /// Tag for system scoped container
        /// </summary>
        public static string SystemTag = "__System__";

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationServicesBuilder" /> class.
        /// </summary>
        /// <param name="scanner"></param>
        /// <param name="assemblyProvider">The assembly provider.</param>
        /// <param name="assemblyCandidateFinder">The assembly candidate finder.</param>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="environment"></param>
        public ApplicationServicesBuilder(
            IConventionScanner scanner,
            IAssemblyProvider assemblyProvider,
            IAssemblyCandidateFinder assemblyCandidateFinder,
            IServiceCollection services,
            IConfiguration configuration,
            IHostingEnvironment environment)
        {
            _scanner = scanner ?? throw new ArgumentNullException(nameof(scanner));

            AssemblyProvider = assemblyProvider ?? throw new ArgumentNullException(nameof(assemblyProvider));
            AssemblyCandidateFinder = assemblyCandidateFinder ?? throw new ArgumentNullException(nameof(AssemblyCandidateFinder));
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            Environment = environment ?? throw new ArgumentNullException(nameof(environment));

            Services = services;
            _onBuild = new ServiceProviderObservable();
            _application = new ServiceWrapper();
            _system = new ServiceWrapper();
        }

        /// <summary>
        /// Builds the root container, and returns the lifetime scopes for the application and system containers
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        public (IServiceProvider Application, IServiceProvider System) Build(ILogger logger)
        {
            new ConventionComposer(_scanner, logger)
                .Register(this, typeof(IServiceConvention), typeof(ServiceConventionDelegate));

            var applicationServices = new ServiceCollection();
            foreach (var s in Services) applicationServices.Add(s);
            foreach (var s in Application.Services) applicationServices.Add(s);
            var application = applicationServices.BuildServiceProvider();
            _onBuild.Send(application);
            _application.OnBuild.Send(application);

            foreach (var s in System.Services) Services.Add(s);
            var system = Services.BuildServiceProvider();
            _system.OnBuild.Send(system);

            return (application, system);
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }
        public IAssemblyProvider AssemblyProvider { get; }
        public IAssemblyCandidateFinder AssemblyCandidateFinder { get; }
        public IServiceWrapper Application => _application;
        public IServiceWrapper System => _system;

        public IServiceCollection Services { get; }
        public IObservable<IServiceProvider> OnBuild => _onBuild;

        public IServicesBuilder AddDelegate(ServiceConventionDelegate @delegate)
        {
            _scanner.AddDelegate(@delegate);
            return this;
        }

        public IServicesBuilder AddConvention(IServiceConvention convention)
        {
            _scanner.AddConvention(convention);
            return this;
        }
    }
}
