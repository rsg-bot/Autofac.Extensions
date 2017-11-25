using System;
using System.Collections.Generic;
using Autofac;

namespace Rocket.Surgery.Extensions.Autofac.Internals
{
    class LifetimeScopeObservable : IObservable<ILifetimeScope>
    {

        private readonly List<IObserver<ILifetimeScope>> _observers = new List<IObserver<ILifetimeScope>>();

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
                catch
                {
                }
            }
        }
    }
}
