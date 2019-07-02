using System;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Extensions.DependencyInjection;

namespace Rocket.Surgery.Extensions.Autofac
{
    /// <summary>
    /// IAutofacConventionContext
    /// Implements the <see cref="IConventionContext" />
    /// </summary>
    /// <seealso cref="IConventionContext" />
    public interface IAutofacConventionContext : IConventionContext
    {
        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>The configuration.</value>
        IConfiguration Configuration { get; }
        /// <summary>
        /// Gets the assembly provider.
        /// </summary>
        /// <value>The assembly provider.</value>
        IAssemblyProvider AssemblyProvider { get; }
        /// <summary>
        /// Gets the assembly candidate finder.
        /// </summary>
        /// <value>The assembly candidate finder.</value>
        IAssemblyCandidateFinder AssemblyCandidateFinder { get; }
        /// <summary>
        /// Configures the container.
        /// </summary>
        /// <param name="builder">The builder.</param>
        void ConfigureContainer(ContainerBuilderDelegate builder);
        /// <summary>
        /// Gets the services.
        /// </summary>
        /// <value>The services.</value>
        IServiceCollection Services { get; }
        /// <summary>
        /// Gets the on container build.
        /// </summary>
        /// <value>The on container build.</value>
        IObservable<IContainer> OnContainerBuild { get; }

        /// <summary>
        /// The environment that this convention is running
        /// Based on IHostEnvironment / IHostingEnvironment
        /// </summary>
        /// <value>The environment.</value>
        IRocketEnvironment Environment { get; }
    }
}
