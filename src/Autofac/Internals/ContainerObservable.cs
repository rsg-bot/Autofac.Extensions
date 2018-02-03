using System;
using System.Collections.Generic;
using Autofac;
using Microsoft.Extensions.Logging;

namespace Rocket.Surgery.Extensions.Autofac.Internals
{
    class ContainerObservable : IObservable<IContainer>
    {
        private readonly ILogger _logger;

        private readonly List<IObserver<IContainer>> _observers = new List<IObserver<IContainer>>();

        public ContainerObservable(ILogger logger)
        {
            _logger = logger;
        }

        public IDisposable Subscribe(IObserver<IContainer> observer)
        {
            _observers.Add(observer);
            return new Disposable(() =>
            {
                _observers.RemoveAll(x => x == observer);
            });
        }

        public void Send(IContainer value)
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
