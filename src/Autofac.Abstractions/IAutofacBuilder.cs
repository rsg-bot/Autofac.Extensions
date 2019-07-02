using System;
using Autofac;
using Microsoft.Extensions.Configuration;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Extensions.DependencyInjection;

namespace Rocket.Surgery.Extensions.Autofac
{
    /// <summary>
    /// IAutofacBuilder.
    /// Implements the <see cref="IConventionBuilder{IAutofacBuilder, IAutofacConvention, AutofacConventionDelegate}" />
    /// </summary>
    /// <seealso cref="IConventionBuilder{IAutofacBuilder, IAutofacConvention, AutofacConventionDelegate}" />
    public interface IAutofacBuilder : IConventionBuilder<IAutofacBuilder, IAutofacConvention, AutofacConventionDelegate>
    {
    }
}
