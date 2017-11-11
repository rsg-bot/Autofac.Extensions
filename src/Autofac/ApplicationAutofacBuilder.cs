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
            IAssemblyProvider assemblyProvider,
            IAssemblyCandidateFinder assemblyCandidateFinder,
            IConventionScanner scanner,
            IServiceCollection services,
            IConfiguration configuration,
            IServicesEnvironment environment) :
            base(assemblyProvider, assemblyCandidateFinder, scanner, services, configuration, environment)
        { }

        /// <summary>
        /// Builds the root container, and returns the lifetime scopes for the application and system containers
        /// </summary>
        /// <param name="containerBuilder"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public (IContainer Container, ILifetimeScope Application, ILifetimeScope System) Build(ContainerBuilder containerBuilder, ILogger logger)
        {
            new ConventionComposer(_scanner, logger)
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

            var application = container.BeginLifetimeScope(ApplicationTag, a =>
            {
                _application.Collection.Apply(a);
                CustomRegistration.Register(a, _application.Services, ApplicationTag);
            });

            return (container, application, system);
        }
    }
}
