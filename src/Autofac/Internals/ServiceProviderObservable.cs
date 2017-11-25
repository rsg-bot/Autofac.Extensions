using System;
using System.Collections.Generic;

namespace Rocket.Surgery.Extensions.Autofac.Internals
{
    class ServiceProviderObservable : IObservable<IServiceProvider>
    {
        private readonly List<IObserver<IServiceProvider>> _observers = new List<IObserver<IServiceProvider>>();

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
                catch
                {
                }
            }
        }
    }
}
