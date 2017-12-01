using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Builders;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Extensions.DependencyInjection;
using Rocket.Surgery.Hosting;

namespace Rocket.Surgery.Extensions.Autofac
{
    /// <summary>
    /// Class ApplicationAutofacBuilder.
    /// </summary>
    /// <seealso cref="Builder" />
    /// <seealso cref="Microsoft.Extensions.Configuration.IConfigurationBuilder" />
    /// TODO Edit XML Comment Template for ApplicationAutofacBuilder
    public class ApplicationAutofacBuilder : AutofacBuilderBase
    {
        /// <summary>
        /// Tag for applicaiton scoped container
        /// </summary>
        public static string ApplicationTag = "__Application__";
        /// <summary>
        /// Tag for system scoped container
        /// </summary>
        public static string SystemTag = "__System__";


        public ApplicationAutofacBuilder(
            IConventionScanner scanner,
            IAssemblyProvider assemblyProvider,
            IAssemblyCandidateFinder assemblyCandidateFinder,
            IServiceCollection services,
            IConfiguration configuration,
            IHostingEnvironment environment,
            ILogger logger) :
            base(scanner, assemblyProvider, assemblyCandidateFinder, services, configuration, environment, logger)
        { }

        /// <summary>
        /// Builds the root container, and returns the lifetime scopes for the application and system containers
        /// </summary>
        /// <param name="containerBuilder"></param>
        /// <returns></returns>
        public (IContainer Container, ILifetimeScope Application, ILifetimeScope System) Build(ContainerBuilder containerBuilder)
        {
            new ConventionComposer(_scanner)
                .Register(
                    this,
                    typeof(IServiceConvention),
                    typeof(IAutofacConvention),
                    typeof(ServiceConventionDelegate),
                    typeof(AutofacConventionDelegate));

            _core.Collection.Apply(containerBuilder);
            containerBuilder.Populate(Services);

            var container = containerBuilder.Build();

            var system = container.BeginLifetimeScope(SystemTag, s =>
            {
                _system.Collection.Apply(s);
                CustomRegistration.Register(s, _system.Services, SystemTag);
            });
            _system.LifetimeScopeOnBuild.Send(system);
            _system.ServiceProviderOnBuild.Send(new AutofacServiceProvider(system));

            var application = container.BeginLifetimeScope(ApplicationTag, a =>
            {
                _application.Collection.Apply(a);
                CustomRegistration.Register(a, _application.Services, ApplicationTag);
            });
            _application.LifetimeScopeOnBuild.Send(application);
            _application.ServiceProviderOnBuild.Send(new AutofacServiceProvider(application));

            _core.LifetimeScopeOnBuild.Send(container);
            _core.ServiceProviderOnBuild.Send(new AutofacServiceProvider(container));
            _containerObservable.Send(container);

            return (container, application, system);
        }
    }
}
