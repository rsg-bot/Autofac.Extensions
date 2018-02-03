using System;
using System.Collections.Generic;
using Autofac;
using Microsoft.Extensions.Logging;

namespace Rocket.Surgery.Extensions.Autofac.Internals
{
    class LifetimeScopeObservable : IObservable<ILifetimeScope>
    {
        private readonly ILogger _logger;
        private readonly List<IObserver<ILifetimeScope>> _observers = new List<IObserver<ILifetimeScope>>();

        public LifetimeScopeObservable(ILogger logger)
        {
            _logger = logger;
        }

        public IDisposable Subscribe(IObserver<ILifetimeScope> observer)
        {
            _observers.Add(observer);
            return new Disposable(() =>
            {
                _observers.RemoveAll(x => x == observer);
            });
        }

        public void Send(ILifetimeScope value)
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
