using Rocket.Surgery.Builders;

namespace Rocket.Surgery.Extensions.Autofac
{
    /// <summary>
    /// Class IServicesBuilder.
    /// </summary>
    /// TODO Edit XML Comment Template for IServicesBuilder
    public interface IServicesBuilder : IBuilder
    {
        IServicesBuilderConventionItem System { get; }
        IServicesBuilderConventionItem Application { get; }
        IServicesBuilder AddDelegate(ServiceConventionDelegate @delegate);
        IServicesBuilder AddConvention(IServiceConvention @delegate);
    }
}
