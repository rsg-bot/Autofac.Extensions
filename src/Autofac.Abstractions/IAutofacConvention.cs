using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.Extensions.Autofac
{
    /// <summary>
    ///  IAutofacConvention
    /// Implements the <see cref="Rocket.Surgery.Conventions.IConvention{Rocket.Surgery.Extensions.Autofac.IAutofacConventionContext}" />
    /// </summary>
    /// <seealso cref="Rocket.Surgery.Conventions.IConvention{Rocket.Surgery.Extensions.Autofac.IAutofacConventionContext}" />
    public interface IAutofacConvention : IConvention<IAutofacConventionContext>
    {
    }
}
