using System;
using System.Collections.Generic;
using System.Reflection;
using Autofac;
using Autofac.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.Extensions.Autofac
{
    /// <summary>
    /// Extension methods for registering ASP.NET Core dependencies with Autofac.
    /// </summary>
    public static class CustomRegistration
    {
        /// <summary>Configures the lifecycle on a service registration.</summary>
        /// <typeparam name="TActivatorData">The activator data type.</typeparam>
        /// <typeparam name="TRegistrationStyle">The object registration style.</typeparam>
        /// <param name="registrationBuilder">The registration being built.</param>
        /// <param name="lifecycleKind">The lifecycle specified on the service registration.</param>
        /// <returns>
        /// The <paramref name="registrationBuilder" />, configured with the proper lifetime scope,
        /// and available for additional configuration.
        /// </returns>
        private static IRegistrationBuilder<object, TActivatorData, TRegistrationStyle> ConfigureLifecycle<TActivatorData, TRegistrationStyle>(this IRegistrationBuilder<object, TActivatorData, TRegistrationStyle> registrationBuilder, ServiceLifetime lifecycleKind, string tag)
        {
            switch (lifecycleKind)
            {
                case ServiceLifetime.Singleton:
                    registrationBuilder.InstancePerMatchingLifetimeScope(tag);
                    break;
                case ServiceLifetime.Scoped:
                    registrationBuilder.InstancePerLifetimeScope();
                    break;
                case ServiceLifetime.Transient:
                    registrationBuilder.InstancePerDependency();
                    break;
            }
            return registrationBuilder;
        }

        /// <summary>
        /// Populates the Autofac containerBuilder builder with the set of registered service descriptors.
        /// </summary>
        /// <param name="builder">
        /// The <see cref="T:Autofac.ContainerBuilder" /> into which the registrations should be made.
        /// </param>
        /// <param name="descriptors">
        /// The set of service descriptors to register in the containerBuilder.
        /// </param>
        public static void Register(ContainerBuilder builder, IEnumerable<ServiceDescriptor> descriptors, string tag)
        {
            foreach (var descriptor1 in descriptors)
            {
                var descriptor = descriptor1;
                if ((object)descriptor.ImplementationType != null)
                {
                    if (descriptor.ServiceType.GetTypeInfo().IsGenericTypeDefinition)
                        builder.RegisterGeneric(descriptor.ImplementationType).As(descriptor.ServiceType).ConfigureLifecycle(descriptor.Lifetime, tag);
                    else
                        builder.RegisterType(descriptor.ImplementationType).As(descriptor.ServiceType).ConfigureLifecycle(descriptor.Lifetime, tag);
                }
                else if (descriptor.ImplementationFactory != null)
                {
                    var registration = RegistrationBuilder.ForDelegate(
                            descriptor.ServiceType,
                            (context, parameters) => descriptor.ImplementationFactory(context.Resolve<IServiceProvider>())
                        )
                        .ConfigureLifecycle(descriptor.Lifetime, tag).CreateRegistration();
                    builder.RegisterComponent(registration);
                }
                else
                {
                    var registration = RegistrationBuilder.ForDelegate(
                            descriptor.ServiceType,
                            (context, parameters) => descriptor.ImplementationInstance
                        )
                        .ConfigureLifecycle(descriptor.Lifetime, tag).CreateRegistration();
                    builder.RegisterComponent(registration);
                }
            }
        }
    }
}