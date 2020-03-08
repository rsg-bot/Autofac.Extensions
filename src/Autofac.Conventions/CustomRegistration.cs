using System;
using System.Collections.Generic;
using System.Reflection;
using Autofac;
using Autofac.Builder;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
#pragma warning disable CA2000 // Dispose objects before losing scope

namespace Rocket.Surgery.Extensions.Autofac
{
    /// <summary>
    /// CustomRegistration.
    /// </summary>
    [PublicAPI]
    public static class CustomRegistration
    {
        /// <summary>
        /// Registers the specified builder.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="descriptors">The descriptors.</param>
        /// <param name="tag">The tag.</param>
        public static void Register([NotNull] ContainerBuilder builder, [NotNull] IEnumerable<ServiceDescriptor> descriptors,
                                    [NotNull] string tag)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (descriptors == null)
            {
                throw new ArgumentNullException(nameof(descriptors));
            }

            if (tag == null)
            {
                throw new ArgumentNullException(nameof(tag));
            }

            foreach (var descriptor1 in descriptors)
            {
                var descriptor = descriptor1;
                if (descriptor.ImplementationType != null)
                {
                    if (descriptor.ServiceType.GetTypeInfo().IsGenericTypeDefinition)
                    {
                        builder.RegisterGeneric(descriptor.ImplementationType).As(descriptor.ServiceType)
                           .ConfigureLifecycle(descriptor.Lifetime, tag);
                    }
                    else
                    {
                        builder.RegisterType(descriptor.ImplementationType).As(descriptor.ServiceType)
                           .ConfigureLifecycle(descriptor.Lifetime, tag);
                    }
                }
                else if (descriptor.ImplementationFactory != null)
                {
                    var registration = RegistrationBuilder.ForDelegate(
                            descriptor.ServiceType,
                            (context, parameters)
                                => descriptor.ImplementationFactory(context.Resolve<IServiceProvider>())
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

        private static IRegistrationBuilder<object, TActivatorData, TRegistrationStyle>
            ConfigureLifecycle<TActivatorData, TRegistrationStyle>(
                this IRegistrationBuilder<object, TActivatorData, TRegistrationStyle> registrationBuilder,
                ServiceLifetime lifecycleKind,
                string tag
            )
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
    }
}