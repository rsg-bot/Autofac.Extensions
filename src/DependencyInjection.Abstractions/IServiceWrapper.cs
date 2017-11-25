using System;
using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.Extensions.DependencyInjection
{
    public interface IServiceWrapper
    {
        IServiceCollection Services { get; }
        IObservable<IServiceProvider> OnBuild { get; }
    }
}
