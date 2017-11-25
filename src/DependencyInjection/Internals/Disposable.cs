using System;

namespace Rocket.Surgery.Extensions.DependencyInjection.Internals
{
    class Disposable : IDisposable
    {
        private readonly Action _action;

        public Disposable(Action action)
        {
            _action = action;
        }
        public void Dispose()
        {
            _action();
        }
    }
}
