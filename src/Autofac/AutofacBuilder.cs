using System;
using System.Collections.Generic;
using System.Diagnostics;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Extensions.Autofac.Internals;
using Rocket.Surgery.Extensions.DependencyInjection;

namespace Rocket.Surgery.Extensions.Autofac
{
    /// <summary>
    /// AutofacBuilder.
    /// Implements the <see cref="Rocket.Surgery.Conventions.ConventionBuilder{Rocket.Surgery.Extensions.Autofac.IAutofacBuilder, Rocket.Surgery.Extensions.Autofac.IAutofacConvention, Rocket.Surgery.Extensions.Autofac.AutofacConventionDelegate}" />
    /// Implements the <see cref="Rocket.Surgery.Extensions.Autofac.IAutofacBuilder" />
    /// Implements the <see cref="Rocket.Surgery.Extensions.DependencyInjection.IServicesBuilder" />
    /// Implements the <see cref="Rocket.Surgery.Extensions.Autofac.IAutofacConventionContext" />
    /// </summary>
    /// <seealso cref="Rocket.Surgery.Conventions.ConventionBuilder{Rocket.Surgery.Extensions.Autofac.IAutofacBuilder, Rocket.Surgery.Extensions.Autofac.IAutofacConvention, Rocket.Surgery.Extensions.Autofac.AutofacConventionDelegate}" />
    /// <seealso cref="Rocket.Surgery.Extensions.Autofac.IAutofacBuilder" />
    /// <seealso cref="Rocket.Surgery.Extensions.DependencyInjection.IServicesBuilder" />
    /// <seealso cref="Rocket.Surgery.Extensions.Autofac.IAutofacConventionContext" />
    public class AutofacBuilder : ConventionBuilder<IAutofacBuilder, IAutofacConvention, AutofacConventionDelegate>, IAutofacBuilder, IServicesBuilder, IAutofacConventionContext
    {
        private readonly GenericObservableObservable<IContainer> _containerObservable;
        private readonly GenericObservableObservable<IServiceProvider> _serviceProviderOnBuild;
        private readonly ContainerBuilder _containerBuilder;
        private readonly ContainerBuilderCollection _collection = new ContainerBuilderCollection();

        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacBuilder" /> class.
        /// </summary>
        /// <param name="environment">The environment.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="scanner">The scanner.</param>
        /// <param name="assemblyProvider">The assembly provider.</param>
        /// <param name="assemblyCandidateFinder">The assembly candidate finder.</param>
        /// <param name="services">The services.</param>
        /// <param name="containerBuilder">The container builder.</param>
        /// <param name="diagnosticSource">The diagnostic source</param>
        /// <param name="properties">The properties</param>
        /// <exception cref="ArgumentNullException">environment
        /// or
        /// containerBuilder
        /// or
        /// configuration
        /// or
        /// services</exception>
        public AutofacBuilder(
            IRocketEnvironment environment,
            IConfiguration configuration,
            IConventionScanner scanner,
            IAssemblyProvider assemblyProvider,
            IAssemblyCandidateFinder assemblyCandidateFinder,
            IServiceCollection services,
            ContainerBuilder containerBuilder,
            DiagnosticSource diagnosticSource,
            IDictionary<object, object> properties)
            : base(scanner, assemblyProvider, assemblyCandidateFinder, properties)
        {
            Environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _containerBuilder = containerBuilder ?? throw new ArgumentNullException(nameof(containerBuilder));
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            Services = services ?? throw new ArgumentNullException(nameof(services));
            Logger = new DiagnosticLogger(diagnosticSource);

            _containerObservable = new GenericObservableObservable<IContainer>(Logger);
            _serviceProviderOnBuild = new GenericObservableObservable<IServiceProvider>(Logger);
        }

        /// <summary>
        /// Configures the container.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns>IAutofacConventionContext.</returns>
        public IAutofacConventionContext ConfigureContainer(ContainerBuilderDelegate builder)
        {
            _collection.Add(builder);
            return this;
        }

        void IAutofacConventionContext.ConfigureContainer(ContainerBuilderDelegate builder)
        {
            _collection.Add(builder);
        }

        /// <summary>
        /// Builds this instance.
        /// </summary>
        /// <returns>IContainer.</returns>
        public IContainer Build()
        {
            new ConventionComposer(Scanner)
                .Register(
                this,
                typeof(IServiceConvention),
                typeof(IAutofacConvention),
                typeof(ServiceConventionDelegate),
                typeof(AutofacConventionDelegate));

            _collection.Apply(_containerBuilder);
            _containerBuilder.Populate(Services);

            var result = _containerBuilder.Build();
            var sp = new AutofacServiceProvider(result);

            _serviceProviderOnBuild.Send(sp);
            _containerObservable.Send(result);

            return result;
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>The configuration.</value>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Gets the services.
        /// </summary>
        /// <value>The services.</value>
        public IServiceCollection Services { get; }
        /// <summary>
        /// The environment that this convention is running
        /// Based on IHostEnvironment / IHostingEnvironment
        /// </summary>
        /// <value>The environment.</value>
        public IRocketEnvironment Environment { get; }

        /// <summary>
        /// Gets the on build.
        /// </summary>
        /// <value>The on build.</value>
        /// <inheritdoc />
        public IObservable<IServiceProvider> OnBuild => _serviceProviderOnBuild;

        /// <summary>
        /// Gets the on container build.
        /// </summary>
        /// <value>The on container build.</value>
        /// <inheritdoc />
        public IObservable<IContainer> OnContainerBuild => _containerObservable;

        /// <summary>
        /// A logger that is configured to work with each convention item
        /// </summary>
        /// <value>The logger.</value>
        /// <inheritdoc />
        public ILogger Logger { get; }

        IServicesBuilder IConventionContainer<IServicesBuilder, IServiceConvention, ServiceConventionDelegate>.AppendConvention(params IServiceConvention[] conventions)
        {
            Scanner.AppendConvention(conventions);
            return this;
        }

        IServicesBuilder IConventionContainer<IServicesBuilder, IServiceConvention, ServiceConventionDelegate>.AppendConvention(IEnumerable<IServiceConvention> conventions)
        {
            Scanner.AppendConvention(conventions);
            return this;
        }

        IServicesBuilder IConventionContainer<IServicesBuilder, IServiceConvention, ServiceConventionDelegate>.AppendConvention<T>()
        {
            Scanner.AppendConvention<T>();
            return this;
        }

        IServicesBuilder IConventionContainer<IServicesBuilder, IServiceConvention, ServiceConventionDelegate>.PrependConvention(params IServiceConvention[] conventions)
        {
            Scanner.PrependConvention(conventions);
            return this;
        }

        IServicesBuilder IConventionContainer<IServicesBuilder, IServiceConvention, ServiceConventionDelegate>.PrependConvention(IEnumerable<IServiceConvention> conventions)
        {
            Scanner.PrependConvention(conventions);
            return this;
        }

        IServicesBuilder IConventionContainer<IServicesBuilder, IServiceConvention, ServiceConventionDelegate>.PrependConvention<T>()
        {
            Scanner.PrependConvention<T>();
            return this;
        }

        IServicesBuilder IConventionContainer<IServicesBuilder, IServiceConvention, ServiceConventionDelegate>.PrependDelegate(params ServiceConventionDelegate[] delegates)
        {
            Scanner.PrependDelegate(delegates);
            return this;
        }

        IServicesBuilder IConventionContainer<IServicesBuilder, IServiceConvention, ServiceConventionDelegate>.PrependDelegate(IEnumerable<ServiceConventionDelegate> delegates)
        {
            Scanner.PrependDelegate(delegates);
            return this;
        }

        IServicesBuilder IConventionContainer<IServicesBuilder, IServiceConvention, ServiceConventionDelegate>.AppendDelegate(params ServiceConventionDelegate[] delegates)
        {
            Scanner.AppendDelegate(delegates);
            return this;
        }

        IServicesBuilder IConventionContainer<IServicesBuilder, IServiceConvention, ServiceConventionDelegate>.AppendDelegate(IEnumerable<ServiceConventionDelegate> delegates)
        {
            Scanner.AppendDelegate(delegates);
            return this;
        }

        IServiceProvider IServicesBuilder.Build()
        {
            return new AutofacServiceProvider(Build());
        }
    }
}
