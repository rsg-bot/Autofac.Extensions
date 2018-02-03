using System;
using System.Linq;
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
    public class ServicesBuilder : Builder, IServicesBuilder
    {
        private readonly IConventionScanner _scanner;
        private readonly ServiceProviderObservable _onBuild;
        private readonly ServiceWrapper _application;
        private readonly ServiceWrapper _system;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationServicesBuilder" /> class.
        /// </summary>
        /// <param name="scanner"></param>
        /// <param name="assemblyProvider">The assembly provider.</param>
        /// <param name="assemblyCandidateFinder">The assembly candidate finder.</param>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="environment"></param>
        /// <param name="logger">The logger</param>
        public ServicesBuilder(
            IConventionScanner scanner,
            IAssemblyProvider assemblyProvider,
            IAssemblyCandidateFinder assemblyCandidateFinder,
            IServiceCollection services,
            IConfiguration configuration,
            IHostingEnvironment environment,
            ILogger logger)
        {
            _scanner = scanner ?? throw new ArgumentNullException(nameof(scanner));

            AssemblyProvider = assemblyProvider ?? throw new ArgumentNullException(nameof(assemblyProvider));
            AssemblyCandidateFinder = assemblyCandidateFinder ?? throw new ArgumentNullException(nameof(AssemblyCandidateFinder));
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            Environment = environment ?? throw new ArgumentNullException(nameof(environment));

            Services = services ?? throw new ArgumentNullException(nameof(services));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _onBuild = new ServiceProviderObservable(Logger);
            _application = new ServiceWrapper(Logger);
            _system = new ServiceWrapper(Logger);
        }

        /// <summary>
        /// Builds the root container, and returns the lifetime scopes for the application and system containers
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        public IServiceProvider Build()
        {
            new ConventionComposer(_scanner)
                .Register(this, typeof(IServiceConvention), typeof(ServiceConventionDelegate));

            foreach (var s in Application.Services) Services.Add(s);

            var result = Services.BuildServiceProvider();
            _onBuild.Send(result);
            _application.OnBuild.Send(result);
            return result;
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }
        public IAssemblyProvider AssemblyProvider { get; }
        public IAssemblyCandidateFinder AssemblyCandidateFinder { get; }

        public IServiceWrapper Application => _application;
        public IServiceWrapper System => _system;

        public IServiceCollection Services { get; }
        public ILogger Logger { get; }
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
