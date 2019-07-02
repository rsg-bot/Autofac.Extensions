using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.Extensions.Autofac
{
    /// <summary>
    /// IAutofacConvention
    /// Implements the <see cref="IConvention{IAutofacConventionContext}" />
    /// </summary>
    /// <seealso cref="IConvention{IAutofacConventionContext}" />
    public interface IAutofacConvention : IConvention<IAutofacConventionContext>
    {
    }
}
