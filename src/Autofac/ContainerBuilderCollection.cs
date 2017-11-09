using System.Collections;
using System.Collections.Generic;
using Autofac;

namespace Rocket.Surgery.Extensions.Autofac
{
    /// <summary>
    /// A collection that houses container builder delegates for being applied later
    /// </summary>
    internal class ContainerBuilderCollection : IEnumerable<ContainerBuilderDelegate>
    {
        private readonly List<ContainerBuilderDelegate> _list = new List<ContainerBuilderDelegate>();

        /// <inheritdoc cref="IEnumerable&lt;ContainerBuilderDelegate&gt;"/>
        public IEnumerator<ContainerBuilderDelegate> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        /// <inheritdoc cref="IEnumerable"/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Add a delegate
        /// </summary>
        /// <param name="delegate"></param>
        /// <returns></returns>
        public ContainerBuilderCollection Add(ContainerBuilderDelegate @delegate)
        {
            _list.Add(@delegate);
            return this;
        }

        /// <summary>
        /// Apply the delegates to the builder
        /// </summary>
        /// <param name="builder"></param>
        public void Apply(ContainerBuilder builder)
        {
            foreach (var item in _list)
            {
                item(builder);
            }
        }
    }
}
