using System;
using System.Collections.Generic;
using Autofac;

namespace Rocket.Surgery.Extensions.Autofac.Internals
{
    class ContainerObservable : IObservable<IContainer>
    {

        private readonly List<IObserver<IContainer>> _observers = new List<IObserver<IContainer>>();

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
                catch
                {
                }
            }
        }
    }
}
