using System.Collections.Generic;
using System.Reflection;
using Rocket.Surgery.Conventions.Reflection;

namespace Rocket.Surgery.Extensions.DependencyInjection.Tests
{
    class TestAssemblyProvider : IAssemblyProvider
    {
        public IEnumerable<Assembly> GetAssemblies()
        {
            return new[]
            {
                typeof(ServicesBuilder).GetTypeInfo().Assembly,
                typeof(TestAssemblyProvider).GetTypeInfo().Assembly
            };
        }
    }
}