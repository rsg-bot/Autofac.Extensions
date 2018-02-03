using System;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Builders;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Extensions.Autofac.Internals;
using Rocket.Surgery.Extensions.DependencyInjection;
using Rocket.Surgery.Hosting;

namespace Rocket.Surgery.Extensions.Autofac
{
    public class AutofacBuilderBase : Builder, IAutofacBuilder, IServicesBuilder
    {
        public ILogger Logger { get; }
        internal readonly IConventionScanner _scanner;
        internal readonly ServiceAndContainerWrapper _core;
        internal readonly ServiceAndContainerWrapper _system;
        internal readonly ServiceAndContainerWrapper _application;
        internal readonly ContainerObservable _containerObservable;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationAutofacBuilder" /> class.
        /// </summary>
        /// <param name="scanner"></param>
        /// <param name="assemblyProvider">The assembly provider.</param>
        /// <param name="assemblyCandidateFinder">The assembly candidate finder.</param>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="environment"></param>
        /// <param name="logger">The logger</param>
        protected AutofacBuilderBase(
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
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _core = new ServiceAndContainerWrapper(this, services);
            _application = new ServiceAndContainerWrapper(this);
            _system = new ServiceAndContainerWrapper(this);
            _containerObservable = new ContainerObservable(Logger);
        }

        public void ConfigureContainer(ContainerBuilderDelegate builder)
        {
            _core.ConfigureContainer(builder);
        }

        public IServiceCollection Services => _core.Services;


        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }
        public IAssemblyProvider AssemblyProvider { get; }
        public IAssemblyCandidateFinder AssemblyCandidateFinder { get; }

        public IAutofacContextWrapper System => _system;
        public IAutofacContextWrapper Application => _application;
        public IObservable<ILifetimeScope> OnBuild => _core.LifetimeScopeOnBuild;
        IObservable<IServiceProvider> IServiceWrapper.OnBuild => _core.ServiceProviderOnBuild;
        public IObservable<IContainer> OnContainerBuild => _containerObservable;

        public IAutofacBuilder AddDelegate(AutofacConventionDelegate @delegate)
        {
            _scanner.AddDelegate(@delegate);
            return this;
        }

        public IAutofacBuilder AddConvention(IAutofacConvention convention)
        {
            _scanner.AddConvention(convention);
            return this;
        }

        // IServiceConventionContext
        IServiceWrapper IServiceConventionContext.System => _system;

        IServiceWrapper IServiceConventionContext.Application => _application;

        IServicesBuilder IServicesBuilder.AddDelegate(ServiceConventionDelegate @delegate)
        {
            _scanner.AddDelegate(@delegate);
            return this;
        }

        IServicesBuilder IServicesBuilder.AddConvention(IServiceConvention convention)
        {
            _scanner.AddConvention(convention);
            return this;
        }
    }
}
