using System;
using Autofac;
using Microsoft.Extensions.Configuration;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Extensions.DependencyInjection;

namespace Rocket.Surgery.Extensions.Autofac
{
    /// <summary>
    /// Class IAutofacBuilder.
    /// </summary>
    /// TODO Edit XML Comment Template for IAutofacBuilder
    public interface IAutofacBuilder : IConventionBuilder<IAutofacBuilder, IAutofacConvention, AutofacConventionDelegate>
    {
    }
}
