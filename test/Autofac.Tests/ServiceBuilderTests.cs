using System;
using Autofac;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Extensions.Autofac.Tests;
using Xunit;

[assembly: Convention(typeof(ServiceBuilderTests.AbcConvention))]

namespace Rocket.Surgery.Extensions.Autofac.Tests
{
    public class ServiceBuilderTests
    {
        [Fact]
        public void Constructs()
        {
            var assemblyProvider = new TestAssemblyProvider();
            var assemblyCandidateFinder = A.Fake<IAssemblyCandidateFinder>();
            var scanner = A.Fake<IConventionScanner>();
            var serviceCollection = new ServiceCollection();
            var configuration = A.Fake<IConfiguration>();
            var servicesBuilder = new ServicesBuilder(assemblyProvider, assemblyCandidateFinder, scanner, serviceCollection, configuration, A.Fake<IServicesEnvironment>());

            servicesBuilder.AssemblyProvider.Should().BeSameAs(assemblyProvider);
            servicesBuilder.AssemblyCandidateFinder.Should().NotBeNull();
            servicesBuilder.Services.Should().BeSameAs(serviceCollection);
            servicesBuilder.Configuration.Should().BeSameAs(configuration);
            servicesBuilder.Application.Should().NotBeNull();
            servicesBuilder.System.Should().NotBeNull();
            servicesBuilder.Environment.Should().NotBeNull();
            Action a = () => { servicesBuilder.AddConvention(A.Fake<IServiceConvention>()); };
            a.Should().NotThrow();
            a = () => { servicesBuilder.AddDelegate(delegate { }); };
            a.Should().NotThrow();
            a = () => { servicesBuilder.Container(delegate { }); };
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
            var servicesBuilder = new ServicesBuilder(assemblyProvider, assemblyCandidateFinder, scanner, serviceCollection, configuration, A.Fake<IServicesEnvironment>());

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
            var servicesBuilder = new ServicesBuilder(assemblyProvider, assemblyCandidateFinder, scanner, serviceCollection, configuration, A.Fake<IServicesEnvironment>());

            servicesBuilder[string.Empty].Should().BeNull();
        }

        [Fact]
        public void AddConventions()
        {
            var assemblyProvider = new TestAssemblyProvider();
            var assemblyCandidateFinder = A.Fake<IAssemblyCandidateFinder>();
            var configuration = A.Fake<IConfiguration>();
            var scanner = A.Fake<IConventionScanner>();
            var serviceCollection = new ServiceCollection();
            var servicesBuilder = new ServicesBuilder(assemblyProvider, assemblyCandidateFinder, scanner, serviceCollection, configuration, A.Fake<IServicesEnvironment>());

            var Convention = A.Fake<IServiceConvention>();

            servicesBuilder.AddConvention(Convention);

            A.CallTo(() => scanner.AddConvention(Convention)).MustHaveHappened();
        }

        public interface Abc { }
        public interface Abc2 { }
        public interface Abc3 { }
        public interface Abc4 { }

        public class AbcConvention : IServiceConvention
        {
            public void Register(IServiceConventionContext context)
            {
                context.Container(c => c.RegisterInstance(A.Fake<Abc>()));
                context.Services.AddSingleton(A.Fake<Abc2>());
                context.System.Container(c => c.RegisterInstance(A.Fake<Abc3>()));
                context.Application.Container(c => { });

                context.AddConvention(A.Fake<IServiceConvention>());
                context.AddDelegate(delegate {  });
            }
        }

        [Fact]
        public void ConstructTheContainerAndRegisterWithCore()
        {
            var assemblyProvider = new TestAssemblyProvider();
            var assemblyCandidateFinder = A.Fake<IAssemblyCandidateFinder>();
            var configuration = A.Fake<IConfiguration>();
            var scanner = A.Fake<IConventionScanner>();
            var serviceCollection = new ServiceCollection();
            var servicesBuilder = new ServicesBuilder(assemblyProvider, assemblyCandidateFinder, scanner, serviceCollection, configuration, A.Fake<IServicesEnvironment>());
            servicesBuilder.Container(c => c.RegisterInstance(A.Fake<Abc>()));
            servicesBuilder.Services.AddSingleton(A.Fake<Abc2>());
            servicesBuilder.System.Container(c => c.RegisterInstance(A.Fake<Abc3>()));

            var items = servicesBuilder.Build(new ContainerBuilder(), A.Fake<ILogger>());
            items.ResolveOptional<Abc>().Should().NotBeNull();
            items.ResolveOptional<Abc2>().Should().NotBeNull();
            items.ResolveOptional<Abc3>().Should().BeNull();
            items.ResolveOptional<Abc4>().Should().BeNull();
        }

        [Fact]
        public void ConstructTheContainerAndRegisterWithApplication()
        {
            var assemblyProvider = new TestAssemblyProvider();
            var assemblyCandidateFinder = A.Fake<IAssemblyCandidateFinder>();
            var configuration = A.Fake<IConfiguration>();
            var scanner = A.Fake<IConventionScanner>();
            var serviceCollection = new ServiceCollection();
            var servicesBuilder = new ServicesBuilder(assemblyProvider, assemblyCandidateFinder, scanner, serviceCollection, configuration, A.Fake<IServicesEnvironment>());
            servicesBuilder.Application.Container(c => c.RegisterInstance(A.Fake<Abc>()));
            servicesBuilder.Application.Services.AddSingleton(A.Fake<Abc2>());
            servicesBuilder.System.Container(c => c.RegisterInstance(A.Fake<Abc3>()));
            servicesBuilder.Container(c => c.RegisterInstance(A.Fake<Abc4>()));

            var items = servicesBuilder.Build(new ContainerBuilder(), A.Fake<ILogger>());
            items.ResolveOptional<Abc>().Should().NotBeNull();
            items.ResolveOptional<Abc2>().Should().NotBeNull();
            items.ResolveOptional<Abc3>().Should().BeNull();
            items.ResolveOptional<Abc4>().Should().NotBeNull();
        }

        [Fact]
        public void ConstructTheContainerAndRegisterWithSystem()
        {
            var assemblyProvider = new TestAssemblyProvider();
            var assemblyCandidateFinder = A.Fake<IAssemblyCandidateFinder>();
            var configuration = A.Fake<IConfiguration>();
            var scanner = A.Fake<IConventionScanner>();
            var serviceCollection = new ServiceCollection();
            var servicesBuilder = new ServicesBuilder(assemblyProvider, assemblyCandidateFinder, scanner, serviceCollection, configuration, A.Fake<IServicesEnvironment>());
            servicesBuilder.System.Container(c => c.RegisterInstance(A.Fake<Abc>()));
            servicesBuilder.System.Services.AddSingleton(A.Fake<Abc2>());
            servicesBuilder.Application.Container(c => c.RegisterInstance(A.Fake<Abc3>()));
            servicesBuilder.Container(c => c.RegisterInstance(A.Fake<Abc4>()));

            var items = servicesBuilder.Build(new ContainerBuilder(), A.Fake<ILogger>());
            items.ResolveOptional<Abc>().Should().BeNull();
            items.ResolveOptional<Abc2>().Should().BeNull();
            items.ResolveOptional<Abc3>().Should().NotBeNull();
            items.ResolveOptional<Abc4>().Should().NotBeNull();
        }

        [Fact]
        public void ConstructTheContainerAndRegisterWithCore_ServiceProvider()
        {
            var assemblyProvider = new TestAssemblyProvider();
            var assemblyCandidateFinder = A.Fake<IAssemblyCandidateFinder>();
            var configuration = A.Fake<IConfiguration>();
            var scanner = A.Fake<IConventionScanner>();
            var serviceCollection = new ServiceCollection();
            var servicesBuilder = new ServicesBuilder(assemblyProvider, assemblyCandidateFinder, scanner, serviceCollection, configuration, A.Fake<IServicesEnvironment>());
            servicesBuilder.Container(c => c.RegisterInstance(A.Fake<Abc>()));
            servicesBuilder.Services.AddSingleton(A.Fake<Abc2>());
            servicesBuilder.System.Container(c => c.RegisterInstance(A.Fake<Abc3>()));

            var items = servicesBuilder.Build(new ContainerBuilder(), A.Fake<ILogger>());

            var sp = items.Resolve<IServiceProvider>();
            sp.GetService<Abc>().Should().NotBeNull();
            sp.GetService<Abc2>().Should().NotBeNull();
            sp.GetService<Abc3>().Should().BeNull();
            sp.GetService<Abc4>().Should().BeNull();
        }

        [Fact]
        public void ConstructTheContainerAndRegisterWithApplication_ServiceProvider()
        {
            var assemblyProvider = new TestAssemblyProvider();
            var assemblyCandidateFinder = A.Fake<IAssemblyCandidateFinder>();
            var configuration = A.Fake<IConfiguration>();
            var scanner = A.Fake<IConventionScanner>();
            var serviceCollection = new ServiceCollection();
            var servicesBuilder = new ServicesBuilder(assemblyProvider, assemblyCandidateFinder, scanner, serviceCollection, configuration, A.Fake<IServicesEnvironment>());
            servicesBuilder.Application.Container(c => c.RegisterInstance(A.Fake<Abc>()));
            servicesBuilder.Application.Services.AddSingleton(A.Fake<Abc2>());
            servicesBuilder.System.Container(c => c.RegisterInstance(A.Fake<Abc3>()));
            servicesBuilder.Container(c => c.RegisterInstance(A.Fake<Abc4>()));

            var items = servicesBuilder.Build(new ContainerBuilder(), A.Fake<ILogger>());
            var sp = items.Resolve<IServiceProvider>();
            sp.GetService<Abc>().Should().NotBeNull();
            sp.GetService<Abc2>().Should().NotBeNull();
            sp.GetService<Abc3>().Should().BeNull();
            sp.GetService<Abc4>().Should().NotBeNull();
        }

        [Fact]
        public void ConstructTheContainerAndRegisterWithSystem_ServiceProvider()
        {
            var assemblyProvider = new TestAssemblyProvider();
            var assemblyCandidateFinder = A.Fake<IAssemblyCandidateFinder>();
            var configuration = A.Fake<IConfiguration>();
            var scanner = A.Fake<IConventionScanner>();
            var serviceCollection = new ServiceCollection();
            var servicesBuilder = new ServicesBuilder(assemblyProvider, assemblyCandidateFinder, scanner, serviceCollection, configuration, A.Fake<IServicesEnvironment>());
            servicesBuilder.System.Container(c => c.RegisterInstance(A.Fake<Abc>()));
            servicesBuilder.System.Services.AddSingleton(A.Fake<Abc2>());
            servicesBuilder.Application.Container(c => c.RegisterInstance(A.Fake<Abc3>()));
            servicesBuilder.Container(c => c.RegisterInstance(A.Fake<Abc4>()));

            var items = servicesBuilder.Build(new ContainerBuilder(), A.Fake<ILogger>());
            var sp = items.Resolve<IServiceProvider>();
            sp.GetService<Abc>().Should().BeNull();
            sp.GetService<Abc2>().Should().BeNull();
            sp.GetService<Abc3>().Should().NotBeNull();
            sp.GetService<Abc4>().Should().NotBeNull();
        }

        [Fact]
        public void ConstructTheContainerAndRegisterWithSystem_UsingConvention()
        {
            var assemblyProvider = new TestAssemblyProvider();
            var assemblyCandidateFinder = A.Fake<IAssemblyCandidateFinder>();
            var configuration = A.Fake<IConfiguration>();
            var scanner = new AggregateConventionScanner(assemblyCandidateFinder);
            var serviceCollection = new ServiceCollection();
            var servicesBuilder = new ServicesBuilder(assemblyProvider, assemblyCandidateFinder, scanner, serviceCollection, configuration, A.Fake<IServicesEnvironment>());

            A.CallTo(() => assemblyCandidateFinder.GetCandidateAssemblies(A<string[]>._))
                .Returns(assemblyProvider.GetAssemblies());

            var items = servicesBuilder.Build(new ContainerBuilder(), A.Fake<ILogger>());
            items.ResolveOptional<Abc>().Should().NotBeNull();
            items.ResolveOptional<Abc2>().Should().NotBeNull();
            items.ResolveOptional<Abc3>().Should().BeNull();
            items.ResolveOptional<Abc4>().Should().BeNull();
        }
    }
}
