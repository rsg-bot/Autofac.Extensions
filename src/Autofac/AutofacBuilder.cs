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
    public class AutofacBuilder : ConventionBuilder<IAutofacBuilder, IAutofacConvention, AutofacConventionDelegate>, IAutofacBuilder, IServicesBuilder, IAutofacConventionContext
    {
        private readonly GenericObservableObservable<IContainer> _containerObservable;
        private readonly GenericObservableObservable<IServiceProvider> _serviceProviderOnBuild;
        private readonly ContainerBuilder _containerBuilder;
        private readonly ContainerBuilderCollection _collection = new ContainerBuilderCollection();

        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacBuilder" /> class.
        /// </summary>
        /// <param name="containerBuilder"></param>
        /// <param name="scanner"></param>
        /// <param name="assemblyProvider">The assembly provider.</param>
        /// <param name="assemblyCandidateFinder">The assembly candidate finder.</param>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="environment"></param>
        /// <param name="diagnosticSource">The diagnostic source</param>
        /// <param name="properties">The properties</param>
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
            : base(environment, scanner, assemblyProvider, assemblyCandidateFinder, properties)
        {
            _containerBuilder = containerBuilder ?? throw new ArgumentNullException(nameof(containerBuilder));
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            Services = services ?? throw new ArgumentNullException(nameof(services));
            Logger = new DiagnosticLogger(diagnosticSource);

            _containerObservable = new GenericObservableObservable<IContainer>(Logger);
            _serviceProviderOnBuild = new GenericObservableObservable<IServiceProvider>(Logger);
        }

        public IAutofacConventionContext ConfigureContainer(ContainerBuilderDelegate builder)
        {
            _collection.Add(builder);
            return this;
        }

        void IAutofacConventionContext.ConfigureContainer(ContainerBuilderDelegate builder)
        {
            _collection.Add(builder);
        }

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

        public IConfiguration Configuration { get; }

        public IServiceCollection Services { get; }

        /// <inheritdoc />
        public IObservable<IServiceProvider> OnBuild => _serviceProviderOnBuild;

        /// <inheritdoc />
        public IObservable<IContainer> OnContainerBuild => _containerObservable;

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
