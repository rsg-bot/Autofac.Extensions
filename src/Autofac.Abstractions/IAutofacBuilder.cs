using System;
using Autofac;
using Microsoft.Extensions.Configuration;
using Rocket.Surgery.Builders;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Extensions.DependencyInjection;
using Rocket.Surgery.Hosting;

namespace Rocket.Surgery.Extensions.Autofac
{
    /// <summary>
    /// Class IAutofacBuilder.
    /// </summary>
    /// TODO Edit XML Comment Template for IAutofacBuilder
    public interface IAutofacBuilder : IConventionBuilder<IAutofacBuilder, IAutofacConvention, AutofacConventionDelegate>, IAutofacConventionContext
    {
        IObservable<IContainer> OnContainerBuild { get; }
    }
}
