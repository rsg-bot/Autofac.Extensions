using System;
using Autofac;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Extensions.DependencyInjection;
using Rocket.Surgery.Hosting;
using Xunit;

namespace Rocket.Surgery.Extensions.Autofac.Tests
{
    public class ApplicationServiceBuilderTests
    {
        [Fact]
        public void Constructs()
        {
            var assemblyProvider = new TestAssemblyProvider();
            var assemblyCandidateFinder = A.Fake<IAssemblyCandidateFinder>();
            var scanner = A.Fake<IConventionScanner>();
            var serviceCollection = new ServiceCollection();
            var configuration = A.Fake<IConfiguration>();
            var servicesBuilder = new ApplicationAutofacBuilder(scanner, assemblyProvider, assemblyCandidateFinder, serviceCollection, configuration, A.Fake<IHostingEnvironment>());

            servicesBuilder.AssemblyProvider.Should().BeSameAs(assemblyProvider);
            servicesBuilder.AssemblyCandidateFinder.Should().NotBeNull();
            servicesBuilder.Services.Should().BeSameAs(serviceCollection);
            servicesBuilder.Configuration.Should().BeSameAs(configuration);
            servicesBuilder.Application.Should().NotBeNull();
            servicesBuilder.System.Should().NotBeNull();
            servicesBuilder.Environment.Should().NotBeNull();
            Action a = () => { servicesBuilder.AddConvention(A.Fake<IAutofacConvention>()); };
            a.Should().NotThrow();
            a = () => { servicesBuilder.AddDelegate(delegate { }); };
            a.Should().NotThrow();
            a = () => { servicesBuilder.ConfigureContainer(delegate { }); };
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
            var servicesBuilder = new ApplicationAutofacBuilder(scanner, assemblyProvider, assemblyCandidateFinder, serviceCollection, configuration, A.Fake<IHostingEnvironment>());

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
            var servicesBuilder = new ApplicationAutofacBuilder(scanner, assemblyProvider, assemblyCandidateFinder, serviceCollection, configuration, A.Fake<IHostingEnvironment>());

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
            var servicesBuilder = new ApplicationAutofacBuilder(scanner, assemblyProvider, assemblyCandidateFinder, serviceCollection, configuration, A.Fake<IHostingEnvironment>());

            var Convention = A.Fake<IAutofacConvention>();

            servicesBuilder.AddConvention(Convention);

            A.CallTo(() => scanner.AddConvention(Convention)).MustHaveHappened();
        }

        [Fact]
        public void ConstructTheContainerAndRegisterWithCore()
        {
            var assemblyProvider = new TestAssemblyProvider();
            var assemblyCandidateFinder = A.Fake<IAssemblyCandidateFinder>();
            var configuration = A.Fake<IConfiguration>();
            var scanner = A.Fake<IConventionScanner>();
            var serviceCollection = new ServiceCollection();
            var servicesBuilder = new ApplicationAutofacBuilder(scanner, assemblyProvider, assemblyCandidateFinder, serviceCollection, configuration, A.Fake<IHostingEnvironment>());
            servicesBuilder.ConfigureContainer(c => c.RegisterInstance(A.Fake<ServiceBuilderTests.Abc>()));
            servicesBuilder.Services.AddSingleton(A.Fake<ServiceBuilderTests.Abc2>());
            servicesBuilder.System.ConfigureContainer(c => c.RegisterInstance(A.Fake<ServiceBuilderTests.Abc3>()));

            var items = servicesBuilder.Build(new ContainerBuilder(), A.Fake<ILogger>());
            items.Container.ResolveOptional<ServiceBuilderTests.Abc>().Should().NotBeNull();
            items.Container.ResolveOptional<ServiceBuilderTests.Abc2>().Should().NotBeNull();
            items.Container.ResolveOptional<ServiceBuilderTests.Abc3>().Should().BeNull();
            items.Container.ResolveOptional<ServiceBuilderTests.Abc4>().Should().BeNull();
        }

        [Fact]
        public void ConstructTheContainerAndRegisterWithApplication()
        {
            var assemblyProvider = new TestAssemblyProvider();
            var assemblyCandidateFinder = A.Fake<IAssemblyCandidateFinder>();
            var configuration = A.Fake<IConfiguration>();
            var scanner = A.Fake<IConventionScanner>();
            var serviceCollection = new ServiceCollection();
            var servicesBuilder = new ApplicationAutofacBuilder(scanner, assemblyProvider, assemblyCandidateFinder, serviceCollection, configuration, A.Fake<IHostingEnvironment>());
            servicesBuilder.Application.ConfigureContainer(c => c.RegisterInstance(A.Fake<ServiceBuilderTests.Abc>()));
            servicesBuilder.Application.Services.AddSingleton(A.Fake<ServiceBuilderTests.Abc2>());
            servicesBuilder.System.ConfigureContainer(c => c.RegisterInstance(A.Fake<ServiceBuilderTests.Abc3>()));
            servicesBuilder.ConfigureContainer(c => c.RegisterInstance(A.Fake<ServiceBuilderTests.Abc4>()));

            var items = servicesBuilder.Build(new ContainerBuilder(), A.Fake<ILogger>());
            items.Application.ResolveOptional<ServiceBuilderTests.Abc>().Should().NotBeNull();
            items.Application.ResolveOptional<ServiceBuilderTests.Abc2>().Should().NotBeNull();
            items.Application.ResolveOptional<ServiceBuilderTests.Abc3>().Should().BeNull();
            items.Application.ResolveOptional<ServiceBuilderTests.Abc4>().Should().NotBeNull();
        }

        [Fact]
        public void ConstructTheContainerAndRegisterWithSystem()
        {
            var assemblyProvider = new TestAssemblyProvider();
            var assemblyCandidateFinder = A.Fake<IAssemblyCandidateFinder>();
            var configuration = A.Fake<IConfiguration>();
            var scanner = A.Fake<IConventionScanner>();
            var serviceCollection = new ServiceCollection();
            var servicesBuilder = new ApplicationAutofacBuilder(scanner, assemblyProvider, assemblyCandidateFinder, serviceCollection, configuration, A.Fake<IHostingEnvironment>());
            servicesBuilder.System.ConfigureContainer(c => c.RegisterInstance(A.Fake<ServiceBuilderTests.Abc>()));
            servicesBuilder.System.Services.AddSingleton(A.Fake<ServiceBuilderTests.Abc2>());
            servicesBuilder.Application.ConfigureContainer(c => c.RegisterInstance(A.Fake<ServiceBuilderTests.Abc3>()));
            servicesBuilder.ConfigureContainer(c => c.RegisterInstance(A.Fake<ServiceBuilderTests.Abc4>()));

            var items = servicesBuilder.Build(new ContainerBuilder(), A.Fake<ILogger>());
            items.System.ResolveOptional<ServiceBuilderTests.Abc>().Should().NotBeNull();
            items.System.ResolveOptional<ServiceBuilderTests.Abc2>().Should().NotBeNull();
            items.System.ResolveOptional<ServiceBuilderTests.Abc3>().Should().BeNull();
            items.System.ResolveOptional<ServiceBuilderTests.Abc4>().Should().NotBeNull();
        }

        [Fact]
        public void ConstructTheContainerAndRegisterWithCore_ServiceProvider()
        {
            var assemblyProvider = new TestAssemblyProvider();
            var assemblyCandidateFinder = A.Fake<IAssemblyCandidateFinder>();
            var configuration = A.Fake<IConfiguration>();
            var scanner = A.Fake<IConventionScanner>();
            var serviceCollection = new ServiceCollection();
            var servicesBuilder = new ApplicationAutofacBuilder(scanner, assemblyProvider, assemblyCandidateFinder, serviceCollection, configuration, A.Fake<IHostingEnvironment>());
            servicesBuilder.ConfigureContainer(c => c.RegisterInstance(A.Fake<ServiceBuilderTests.Abc>()));
            servicesBuilder.Services.AddSingleton(A.Fake<ServiceBuilderTests.Abc2>());
            servicesBuilder.System.ConfigureContainer(c => c.RegisterInstance(A.Fake<ServiceBuilderTests.Abc3>()));

            var items = servicesBuilder.Build(new ContainerBuilder(), A.Fake<ILogger>());

            var sp = items.Container.Resolve<IServiceProvider>();
            sp.GetService<ServiceBuilderTests.Abc>().Should().NotBeNull();
            sp.GetService<ServiceBuilderTests.Abc2>().Should().NotBeNull();
            sp.GetService<ServiceBuilderTests.Abc3>().Should().BeNull();
            sp.GetService<ServiceBuilderTests.Abc4>().Should().BeNull();
        }

        [Fact]
        public void ConstructTheContainerAndRegisterWithApplication_ServiceProvider()
        {
            var assemblyProvider = new TestAssemblyProvider();
            var assemblyCandidateFinder = A.Fake<IAssemblyCandidateFinder>();
            var configuration = A.Fake<IConfiguration>();
            var scanner = A.Fake<IConventionScanner>();
            var serviceCollection = new ServiceCollection();
            var servicesBuilder = new ApplicationAutofacBuilder(scanner, assemblyProvider, assemblyCandidateFinder, serviceCollection, configuration, A.Fake<IHostingEnvironment>());
            servicesBuilder.Application.ConfigureContainer(c => c.RegisterInstance(A.Fake<ServiceBuilderTests.Abc>()));
            servicesBuilder.Application.Services.AddSingleton(A.Fake<ServiceBuilderTests.Abc2>());
            servicesBuilder.System.ConfigureContainer(c => c.RegisterInstance(A.Fake<ServiceBuilderTests.Abc3>()));
            servicesBuilder.ConfigureContainer(c => c.RegisterInstance(A.Fake<ServiceBuilderTests.Abc4>()));

            var items = servicesBuilder.Build(new ContainerBuilder(), A.Fake<ILogger>());
            var sp = items.Application.Resolve<IServiceProvider>();
            sp.GetService<ServiceBuilderTests.Abc>().Should().NotBeNull();
            sp.GetService<ServiceBuilderTests.Abc2>().Should().NotBeNull();
            sp.GetService<ServiceBuilderTests.Abc3>().Should().BeNull();
            sp.GetService<ServiceBuilderTests.Abc4>().Should().NotBeNull();
        }

        [Fact]
        public void ConstructTheContainerAndRegisterWithSystem_ServiceProvider()
        {
            var assemblyProvider = new TestAssemblyProvider();
            var assemblyCandidateFinder = A.Fake<IAssemblyCandidateFinder>();
            var configuration = A.Fake<IConfiguration>();
            var scanner = A.Fake<IConventionScanner>();
            var serviceCollection = new ServiceCollection();
            var servicesBuilder = new ApplicationAutofacBuilder(scanner, assemblyProvider, assemblyCandidateFinder, serviceCollection, configuration, A.Fake<IHostingEnvironment>());
            servicesBuilder.System.ConfigureContainer(c => c.RegisterInstance(A.Fake<ServiceBuilderTests.Abc>()));
            servicesBuilder.System.Services.AddSingleton(A.Fake<ServiceBuilderTests.Abc2>());
            servicesBuilder.Application.ConfigureContainer(c => c.RegisterInstance(A.Fake<ServiceBuilderTests.Abc3>()));
            servicesBuilder.ConfigureContainer(c => c.RegisterInstance(A.Fake<ServiceBuilderTests.Abc4>()));

            var items = servicesBuilder.Build(new ContainerBuilder(), A.Fake<ILogger>());
            var sp = items.System.Resolve<IServiceProvider>();
            sp.GetService<ServiceBuilderTests.Abc>().Should().NotBeNull();
            sp.GetService<ServiceBuilderTests.Abc2>().Should().NotBeNull();
            sp.GetService<ServiceBuilderTests.Abc3>().Should().BeNull();
            sp.GetService<ServiceBuilderTests.Abc4>().Should().NotBeNull();
        }

        [Fact]
        public void ConstructTheContainerAndRegisterWithSystem_UsingConvention()
        {
            var assemblyProvider = new TestAssemblyProvider();
            var assemblyCandidateFinder = A.Fake<IAssemblyCandidateFinder>();
            var configuration = A.Fake<IConfiguration>();
            var scanner = new AggregateConventionScanner(assemblyCandidateFinder);
            var serviceCollection = new ServiceCollection();
            var servicesBuilder = new ApplicationAutofacBuilder(scanner, assemblyProvider, assemblyCandidateFinder, serviceCollection, configuration, A.Fake<IHostingEnvironment>());

            A.CallTo(() => assemblyCandidateFinder.GetCandidateAssemblies(A<string[]>._))
                .Returns(assemblyProvider.GetAssemblies());

            var items = servicesBuilder.Build(new ContainerBuilder(), A.Fake<ILogger>());
            items.Container.ResolveOptional<ServiceBuilderTests.Abc>().Should().NotBeNull();
            items.Container.ResolveOptional<ServiceBuilderTests.Abc2>().Should().NotBeNull();
            items.Container.ResolveOptional<ServiceBuilderTests.Abc3>().Should().BeNull();
            items.Container.ResolveOptional<ServiceBuilderTests.Abc4>().Should().BeNull();
        }

        [Fact]
        public void ConstructTheContainerAndRegisterWithSystem_UsingConvention_IncludingOtherBits()
        {
            var assemblyProvider = new TestAssemblyProvider();
            var assemblyCandidateFinder = A.Fake<IAssemblyCandidateFinder>();
            var configuration = A.Fake<IConfiguration>();
            var scanner = new AggregateConventionScanner(assemblyCandidateFinder);
            var serviceCollection = new ServiceCollection();
            var servicesBuilder = new ApplicationAutofacBuilder(scanner, assemblyProvider, assemblyCandidateFinder, serviceCollection, configuration, A.Fake<IHostingEnvironment>());

            A.CallTo(() => assemblyCandidateFinder.GetCandidateAssemblies(A<string[]>._))
                .Returns(assemblyProvider.GetAssemblies());

            var items = servicesBuilder.Build(new ContainerBuilder(), A.Fake<ILogger>());
            items.Container.ResolveOptional<ServiceBuilderTests.Abc>().Should().NotBeNull();
            items.Container.ResolveOptional<ServiceBuilderTests.Abc2>().Should().NotBeNull();
            items.Container.ResolveOptional<ServiceBuilderTests.Abc3>().Should().BeNull();
            items.Container.ResolveOptional<ServiceBuilderTests.Abc4>().Should().BeNull();
            items.Container.ResolveOptional<ServiceBuilderTests.OtherAbc3>().Should().NotBeNull();
            items.Container.ResolveOptional<ServiceBuilderTests.OtherAbc3>().Should().NotBeNull();
        }
    }
}
