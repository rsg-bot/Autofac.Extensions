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

namespace Rocket.Surgery.Extensions.DependencyInjection
{
    public class ServicesBuilder : Builder, IServicesBuilder, IServiceConventionContext
    {
        private readonly IConventionScanner _scanner;

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

            Services= services;
            Application = new ServiceCollection();
            System = new ServiceCollection();
        }

        /// <summary>
        /// Builds the root container, and returns the lifetime scopes for the application and system containers
        /// </summary>
        /// <param name="containerBuilder"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public IServiceProvider Build(ILogger logger)
        {
            new ConventionComposer(_scanner, logger)
                .Register(this, typeof(IServiceConvention), typeof(ServiceConventionDelegate));

            foreach (var s in Application) Services.Add(s);

            return Services.BuildServiceProvider();
        }

        public IConfiguration Configuration { get; }
        public IServicesEnvironment Environment { get; }
        public IAssemblyProvider AssemblyProvider { get; }
        public IAssemblyCandidateFinder AssemblyCandidateFinder { get; }
        public IServiceCollection Application { get; }
        public IServiceCollection Services { get; }
        public IServiceCollection System { get; }

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
