using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Rocket.Surgery.Extensions.Autofac.Internals
{
    class GenericObservableObservable<T> : IObservable<T>
    {
        private readonly ILogger _logger;
        private readonly List<IObserver<T>> _observers = new List<IObserver<T>>();

        public GenericObservableObservable(ILogger logger)
        {
            _logger = logger;
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            _observers.Add(observer);
            return new Disposable(() =>
            {
                _observers.RemoveAll(x => x == observer);
            });
        }

        public void Send(T value)
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
