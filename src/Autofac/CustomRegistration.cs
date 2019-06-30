using System;
using System.Collections.Generic;
using System.Reflection;
using Autofac;
using Autofac.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.Extensions.Autofac
{
    /// <summary>
    /// CustomRegistration.
    /// </summary>
    public static class CustomRegistration
    {
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
        /// Registers the specified builder.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="descriptors">The descriptors.</param>
        /// <param name="tag">The tag.</param>
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
