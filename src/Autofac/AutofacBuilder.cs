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
using Rocket.Surgery.Extensions.DependencyInjection;

namespace Rocket.Surgery.Extensions.Autofac
{
    public class AutofacBuilder : AutofacBuilderBase, IServicesBuilder
    {

        public AutofacBuilder(
            ContainerBuilder containerBuilder,
            IConventionScanner scanner,
            IAssemblyProvider assemblyProvider,
            IAssemblyCandidateFinder assemblyCandidateFinder,
            IServiceCollection services,
            IConfiguration configuration,
            IHostingEnvironment environment,
            DiagnosticSource diagnosticSource,
            IDictionary<object, object> properties) :
            base(containerBuilder, scanner, assemblyProvider, assemblyCandidateFinder, services, configuration, environment, diagnosticSource, properties)
        {
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

            _core.Collection.Apply(_containerBuilder);
            _containerBuilder.Populate(Services);

            _application.Collection.Apply(_containerBuilder);
            _containerBuilder.Populate(_application.Services);

            var result = _containerBuilder.Build();
            var sp = new AutofacServiceProvider(result);
            _application.LifetimeScopeOnBuild.Send(result);
            _application.ServiceProviderOnBuild.Send(sp);

            _core.LifetimeScopeOnBuild.Send(result);
            _core.ServiceProviderOnBuild.Send(sp);

            _containerObservable.Send(result);

            return result;
        }

        IServiceProvider IServicesBuilder.Build()
        {
            return new AutofacServiceProvider(Build());
        }
    }
}
