using System;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Builders;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Extensions.Autofac.Internals;
using Rocket.Surgery.Extensions.DependencyInjection;

namespace Rocket.Surgery.Extensions.Autofac
{
    public class AutofacBuilderBase : ConventionBuilder<IAutofacBuilder, IAutofacConvention, AutofacConventionDelegate>, IAutofacBuilder, IServicesBuilder
    {
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
            ILogger logger) : base(scanner, assemblyProvider, assemblyCandidateFinder)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            Environment = environment ?? throw new ArgumentNullException(nameof(environment));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _core = new ServiceAndContainerWrapper(this, services);
            _application = new ServiceAndContainerWrapper(this);
            _system = new ServiceAndContainerWrapper(this);
            _containerObservable = new ContainerObservable(Logger);
        }

        protected override IAutofacBuilder GetBuilder() => this;

        public void ConfigureContainer(ContainerBuilderDelegate builder)
        {
            _core.ConfigureContainer(builder);
        }

        public IServicesBuilder PrependDelegate(ServiceConventionDelegate @delegate)
        {
            Scanner.PrependDelegate(@delegate);
            return this;
        }

        public IServicesBuilder PrependConvention(IServiceConvention convention)
        {
            Scanner.PrependConvention(convention);
            return this;
        }

        public IServicesBuilder AppendDelegate(ServiceConventionDelegate @delegate)
        {
            Scanner.AppendDelegate(@delegate);
            return this;
        }

        public IServicesBuilder AppendConvention(IServiceConvention convention)
        {
            Scanner.AppendConvention(convention);
            return this;
        }

        public IServiceCollection Services => _core.Services;


        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }

        public IAutofacContextWrapper System => _system;
        public IAutofacContextWrapper Application => _application;
        public IObservable<ILifetimeScope> OnBuild => _core.LifetimeScopeOnBuild;
        IObservable<IServiceProvider> IServiceWrapper.OnBuild => _core.ServiceProviderOnBuild;
        public IObservable<IContainer> OnContainerBuild => _containerObservable;
        public ILogger Logger { get; }

        // IServiceConventionContext
        IServiceWrapper IServiceConventionContext.System => _system;

        IServiceWrapper IServiceConventionContext.Application => _application;
    }
}
