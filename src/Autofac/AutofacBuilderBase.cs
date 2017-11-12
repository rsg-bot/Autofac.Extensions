using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Builders;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Extensions.DependencyInjection;
using Rocket.Surgery.Hosting;

namespace Rocket.Surgery.Extensions.Autofac
{
    public class AutofacBuilderBase : Builder, IAutofacBuilder, IAutofacConventionContext, IServiceConventionContext
    {
        internal readonly IConventionScanner _scanner;
        internal readonly ServiceAndContainerWrapper _core;
        internal readonly ServiceAndContainerWrapper _system;
        internal readonly ServiceAndContainerWrapper _application;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationAutofacBuilder" /> class.
        /// </summary>
        /// <param name="scanner"></param>
        /// <param name="assemblyProvider">The assembly provider.</param>
        /// <param name="assemblyCandidateFinder">The assembly candidate finder.</param>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="environment"></param>
        protected AutofacBuilderBase(
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

            _core = new ServiceAndContainerWrapper(this, services);
            _application = new ServiceAndContainerWrapper(this);
            _system = new ServiceAndContainerWrapper(this);
        }

        public IAutofacBuilder ConfigureContainer(ContainerBuilderDelegate builder)
        {
            _core.ConfigureContainer(builder);
            return this;
        }

        public IServiceCollection Services => _core.Services;
        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }
        public IAssemblyProvider AssemblyProvider { get; }
        public IAssemblyCandidateFinder AssemblyCandidateFinder { get; }
        public IServiceBuilderAndContainerWrapper System => _system;
        public IServiceBuilderAndContainerWrapper Application => _application;

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

        // IAutofacConventionContext
        IServiceAndContainerWrapper IAutofacConventionContext.System => _system;
        IServiceAndContainerWrapper IAutofacConventionContext.Application => _application;
        IAutofacConventionContext IServiceAndContainerWrapper.ConfigureContainer(ContainerBuilderDelegate builder)
        {
            ConfigureContainer(builder);
            return this;
        }

        // IServiceConventionContext
        IServiceCollection IServiceConventionContext.System => _system.Services;

        IServiceCollection IServiceConventionContext.Application => _application.Services;
    }
}
