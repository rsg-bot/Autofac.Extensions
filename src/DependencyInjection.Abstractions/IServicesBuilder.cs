using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Builders;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;

namespace Rocket.Surgery.Extensions.DependencyInjection
{
    /// <summary>
    /// Class IServicesBuilder.
    /// </summary>
    /// TODO Edit XML Comment Template for IServicesBuilder
    public interface IServicesBuilder : IConventionBuilder<IServicesBuilder, IServiceConvention, ServiceConventionDelegate>, IServiceConventionContext { }
}
