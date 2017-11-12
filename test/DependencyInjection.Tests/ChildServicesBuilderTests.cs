using System;
using System.Collections.Generic;
using System.Reflection;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Hosting;
using Xunit;

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

    public class ChildServicesBuilderTests
    {
        class ChildBuilder : ChildServicesBuilder
        {
            public ChildBuilder(IServicesBuilder parent) : base(parent)
            {
            }
        }

        [Fact]
        public void Constructs()
        {
            var assemblyProvider = new TestAssemblyProvider();
            var assemblyCandidateFinder = A.Fake<IAssemblyCandidateFinder>();
            var scanner = A.Fake<IConventionScanner>();
            var serviceCollection = new ServiceCollection();
            var configuration = A.Fake<IConfiguration>();
            var servicesBuilder = new ChildBuilder(new ServicesBuilder(scanner, assemblyProvider, assemblyCandidateFinder, serviceCollection, configuration, A.Fake<IHostingEnvironment>()));

            servicesBuilder.AssemblyProvider.Should().BeSameAs(assemblyProvider);
            servicesBuilder.AssemblyCandidateFinder.Should().NotBeNull();
            servicesBuilder.Services.Should().BeSameAs(serviceCollection);
            servicesBuilder.Configuration.Should().BeSameAs(configuration);
            servicesBuilder.Application.Should().NotBeNull();
            servicesBuilder.System.Should().NotBeNull();
            servicesBuilder.Environment.Should().NotBeNull();
            Action a = () => { servicesBuilder.AddConvention(A.Fake<IServiceConvention>()); };
            a.Should().NotThrow();
            a = () => { servicesBuilder.AddDelegate(delegate {  }); };
            a.Should().NotThrow();
        }

        [Fact]
        public void StoresAndReturnsItems()
        {
            var assemblyProvider = new TestAssemblyProvider();
            var assemblyCandidateFinder = A.Fake<IAssemblyCandidateFinder>();
            var configuration = A.Fake<IConfiguration>();
            var scanner = A.Fake<IConventionScanner>();
            var serviceCollection = new ServiceCollection();
            var servicesBuilder = new ChildBuilder(new ApplicationServicesBuilder(scanner, assemblyProvider, assemblyCandidateFinder, serviceCollection, configuration, A.Fake<IHostingEnvironment>()));

            var value = new object();
            servicesBuilder[string.Empty] = value;
            servicesBuilder[string.Empty].Should().BeSameAs(value);
        }

        [Fact]
        public void IgnoreNonExistentItems()
        {
            var assemblyProvider = new TestAssemblyProvider();
            var assemblyCandidateFinder = A.Fake<IAssemblyCandidateFinder>();
            var configuration = A.Fake<IConfiguration>();
            var scanner = A.Fake<IConventionScanner>();
            var serviceCollection = new ServiceCollection();
            var servicesBuilder = new ChildBuilder(new ServicesBuilder(scanner, assemblyProvider, assemblyCandidateFinder, serviceCollection, configuration, A.Fake<IHostingEnvironment>()));

            servicesBuilder[string.Empty].Should().BeNull();
        }

        [Fact]
        public void Nests()
        {
            var assemblyProvider = new TestAssemblyProvider();
            var assemblyCandidateFinder = A.Fake<IAssemblyCandidateFinder>();
            var configuration = A.Fake<IConfiguration>();
            var scanner = A.Fake<IConventionScanner>();
            var serviceCollection = new ServiceCollection();
            var parent = new ServicesBuilder(scanner, assemblyProvider, assemblyCandidateFinder, serviceCollection, configuration, A.Fake<IHostingEnvironment>());
            var servicesBuilder = new ChildBuilder(parent);

            servicesBuilder[string.Empty].Should().BeNull();

            servicesBuilder.Exit().Should().BeSameAs(parent);
        }
    }
}
