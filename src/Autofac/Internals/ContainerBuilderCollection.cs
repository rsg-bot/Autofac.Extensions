using System.Collections;
using System.Collections.Generic;
using Autofac;

namespace Rocket.Surgery.Extensions.Autofac.Internals
{
    /// <summary>
    /// A collection that houses container builder delegates for being applied later
    /// Implements the <see cref="IEnumerable{ContainerBuilderDelegate}" />
    /// </summary>
    /// <seealso cref="IEnumerable{ContainerBuilderDelegate}" />
    internal class ContainerBuilderCollection : IEnumerable<ContainerBuilderDelegate>
    {
        private readonly List<ContainerBuilderDelegate> _list = new List<ContainerBuilderDelegate>();

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        /// <inheritdoc cref="IEnumerable{ContainerBuilderDelegate}" />
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
        /// <param name="delegate">The delegate.</param>
        /// <returns>ContainerBuilderCollection.</returns>
        public ContainerBuilderCollection Add(ContainerBuilderDelegate @delegate)
        {
            _list.Add(@delegate);
            return this;
        }

        /// <summary>
        /// Apply the delegates to the builder
        /// </summary>
        /// <param name="builder">The builder.</param>
        public void Apply(ContainerBuilder builder)
        {
            foreach (var item in _list)
            {
                item(builder);
            }
        }
    }
}
