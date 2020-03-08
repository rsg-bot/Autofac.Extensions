using JetBrains.Annotations;
using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.Extensions.Autofac
{
    /// <summary>
    /// IAutofacBuilder.
    /// Implements the <see cref="IConventionBuilder{TBuilder,TConvention,TDelegate}" />
    /// </summary>
    /// <seealso cref="IConventionBuilder{IAutofacBuilder, IAutofacConvention, AutofacConventionDelegate}" />
    [PublicAPI]
    public interface
        IAutofacBuilder : IConventionBuilder<IAutofacBuilder, IAutofacConvention, AutofacConventionDelegate> { }
}