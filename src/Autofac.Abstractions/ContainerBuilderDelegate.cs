using Autofac;

namespace Rocket.Surgery.Extensions.Autofac
{
    /// <summary>
    /// A delegate for the autofac container builder
    /// </summary>
    /// <param name="builder">The builder.</param>
    public delegate void ContainerBuilderDelegate(ContainerBuilder builder);
}
