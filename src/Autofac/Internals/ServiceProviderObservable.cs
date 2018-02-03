using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Rocket.Surgery.Extensions.Autofac.Internals
{
    class ServiceProviderObservable : IObservable<IServiceProvider>
    {
        private readonly ILogger _logger;
        private readonly List<IObserver<IServiceProvider>> _observers = new List<IObserver<IServiceProvider>>();

        public ServiceProviderObservable(ILogger logger)
        {
            _logger = logger;
        }

        public IDisposable Subscribe(IObserver<IServiceProvider> observer)
        {
            _observers.Add(observer);
            return new Disposable(() =>
            {
                _observers.RemoveAll(x => x == observer);
            });
        }

        public void Send(IServiceProvider value)
        {
            foreach (var observer in _observers)
            {
                try
                {
                    observer.OnNext(value);
                    observer.OnCompleted();
                }
                catch (Exception e)
                {
                    _logger.LogError(0, e, "Failed to execute observer");
                }
            }
        }
    }
}
