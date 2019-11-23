using System;
using Autofac;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Extensions.Autofac;
using Rocket.Surgery.Hosting;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Hosting
{
    /// <summary>
    /// Class AutofacRocketHostExtensions.
    /// </summary>
    [PublicAPI]
    public static class AutofacRocketHostExtensions
    {
        /// <summary>
        /// Uses the autofac.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="containerBuilder">The container builder.</param>
        /// <returns>IRocketHostBuilder.</returns>
        public static IRocketHostBuilder UseAutofac(
            [NotNull] this IRocketHostBuilder builder,
            ContainerBuilder? containerBuilder = null
        )
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Builder.ConfigureServices(
                (context, services) =>
                {
                    builder.Builder.UseServiceProviderFactory(
                        new ServicesBuilderServiceProviderFactory(
                            collection =>
                                new AutofacBuilder(
                                    context.HostingEnvironment.Convert(),
                                    context.Configuration,
                                    builder.Get<IConventionScanner>(),
                                    builder.Get<IAssemblyProvider>(),
                                    builder.Get<IAssemblyCandidateFinder>(),
                                    collection,
                                    containerBuilder ?? new ContainerBuilder(),
                                    builder.Get<ILogger>(),
                                    builder.ServiceProperties
                                )
                        )
                    );
                }
            );
            return builder;
        }
    }
}