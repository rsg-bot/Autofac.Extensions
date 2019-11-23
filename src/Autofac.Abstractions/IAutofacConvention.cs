using JetBrains.Annotations;
using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.Extensions.Autofac
{
    /// <summary>
    /// IAutofacConvention
    /// Implements the <see cref="IConvention{TContext}" />
    /// </summary>
    /// <seealso cref="IConvention{IAutofacConventionContext}" />
    [PublicAPI]
    public interface IAutofacConvention : IConvention<IAutofacConventionContext> { }
}