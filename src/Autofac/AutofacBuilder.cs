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
            ContainerBuilder containerBuilder,
            IConventionScanner scanner,
            IAssemblyProvider assemblyProvider,
            IAssemblyCandidateFinder assemblyCandidateFinder,
            IServiceCollection services,
            IConfiguration configuration,
            IHostEnvironment environment,
            DiagnosticSource diagnosticSource,
            IDictionary<object, object> properties)
            : base(scanner, assemblyProvider, assemblyCandidateFinder, properties)
        {
            _containerBuilder = containerBuilder ?? throw new ArgumentNullException(nameof(containerBuilder));
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            Environment = environment ?? throw new ArgumentNullException(nameof(environment));
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

        IServiceProvider IServicesBuilder.Build()
        {
            return new AutofacServiceProvider(Build());
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

        protected override IAutofacBuilder GetBuilder()
        {
            return this;
        }

        public IConfiguration Configuration { get; }
        public IHostEnvironment Environment { get; }
        public IServiceCollection Services { get; }

        public IObservable<IServiceProvider> OnBuild => _serviceProviderOnBuild;
        public IObservable<IContainer> OnContainerBuild => _containerObservable;
        public ILogger Logger { get; }
    }
}
