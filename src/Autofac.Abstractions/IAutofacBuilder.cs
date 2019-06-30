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
    /// Implements the <see cref="Rocket.Surgery.Conventions.IConventionBuilder{Rocket.Surgery.Extensions.Autofac.IAutofacBuilder, Rocket.Surgery.Extensions.Autofac.IAutofacConvention, Rocket.Surgery.Extensions.Autofac.AutofacConventionDelegate}" />
    /// </summary>
    /// <seealso cref="Rocket.Surgery.Conventions.IConventionBuilder{Rocket.Surgery.Extensions.Autofac.IAutofacBuilder, Rocket.Surgery.Extensions.Autofac.IAutofacConvention, Rocket.Surgery.Extensions.Autofac.AutofacConventionDelegate}" />
    public interface IAutofacBuilder : IConventionBuilder<IAutofacBuilder, IAutofacConvention, AutofacConventionDelegate>
    {
    }
}
