using System;
using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Hosting;
using Xunit;

namespace Rocket.Surgery.Extensions.DependencyInjection.Tests
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
            var servicesBuilder = new ApplicationServicesBuilder(scanner, assemblyProvider, assemblyCandidateFinder, serviceCollection, configuration, A.Fake<IHostingEnvironment>());

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
        }

        [Fact]
        public void StoresAndReturnsItems()
        {
            var assemblyProvider = new TestAssemblyProvider();
            var assemblyCandidateFinder = A.Fake<IAssemblyCandidateFinder>();
            var configuration = A.Fake<IConfiguration>();
            var scanner = A.Fake<IConventionScanner>();
            var serviceCollection = new ServiceCollection();
            var servicesBuilder = new ApplicationServicesBuilder(scanner, assemblyProvider, assemblyCandidateFinder, serviceCollection, configuration, A.Fake<IHostingEnvironment>());

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
            var servicesBuilder = new ApplicationServicesBuilder(scanner, assemblyProvider, assemblyCandidateFinder, serviceCollection, configuration, A.Fake<IHostingEnvironment>());

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
            var servicesBuilder = new ApplicationServicesBuilder(scanner, assemblyProvider, assemblyCandidateFinder, serviceCollection, configuration, A.Fake<IHostingEnvironment>());

            var Convention = A.Fake<IServiceConvention>();

            servicesBuilder.AddConvention(Convention);

            A.CallTo(() => scanner.AddConvention(Convention)).MustHaveHappened();
        }

        [Fact]
        public void ConstructTheContainerAndRegisterWithCore_ServiceProvider()
        {
            var assemblyProvider = new TestAssemblyProvider();
            var assemblyCandidateFinder = A.Fake<IAssemblyCandidateFinder>();
            var configuration = A.Fake<IConfiguration>();
            var scanner = A.Fake<IConventionScanner>();
            var serviceCollection = new ServiceCollection();
            var servicesBuilder = new ApplicationServicesBuilder(scanner, assemblyProvider, assemblyCandidateFinder, serviceCollection, configuration, A.Fake<IHostingEnvironment>());
            servicesBuilder.Services.AddSingleton(A.Fake<ServiceBuilderTests.Abc>());
            servicesBuilder.Services.AddSingleton(A.Fake<ServiceBuilderTests.Abc2>());
            servicesBuilder.System.Services.AddSingleton(A.Fake<ServiceBuilderTests.Abc3>());

            var items = servicesBuilder.Build(A.Fake<ILogger>());

            var sp = items.Application;
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
            var servicesBuilder = new ApplicationServicesBuilder(scanner, assemblyProvider, assemblyCandidateFinder, serviceCollection, configuration, A.Fake<IHostingEnvironment>());
            servicesBuilder.Application.Services.AddSingleton(A.Fake<ServiceBuilderTests.Abc>());
            servicesBuilder.Application.Services.AddSingleton(A.Fake<ServiceBuilderTests.Abc2>());
            servicesBuilder.System.Services.AddSingleton(A.Fake<ServiceBuilderTests.Abc3>());
            servicesBuilder.Services.AddSingleton(A.Fake<ServiceBuilderTests.Abc4>());

            var items = servicesBuilder.Build(A.Fake<ILogger>());
            var sp = items.Application;
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
            var servicesBuilder = new ApplicationServicesBuilder(scanner, assemblyProvider, assemblyCandidateFinder, serviceCollection, configuration, A.Fake<IHostingEnvironment>());
            servicesBuilder.System.Services.AddSingleton(A.Fake<ServiceBuilderTests.Abc>());
            servicesBuilder.System.Services.AddSingleton(A.Fake<ServiceBuilderTests.Abc2>());
            servicesBuilder.Application.Services.AddSingleton(A.Fake<ServiceBuilderTests.Abc3>());
            servicesBuilder.Services.AddSingleton(A.Fake<ServiceBuilderTests.Abc4>());

            var items = servicesBuilder.Build(A.Fake<ILogger>());
            var sp = items.System;
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
            var servicesBuilder = new ApplicationServicesBuilder(scanner, assemblyProvider, assemblyCandidateFinder, serviceCollection, configuration, A.Fake<IHostingEnvironment>());

            A.CallTo(() => assemblyCandidateFinder.GetCandidateAssemblies(A<IEnumerable<string>>._))
                .Returns(assemblyProvider.GetAssemblies());

            var items = servicesBuilder.Build(A.Fake<ILogger>());
            items.Application.GetService<ServiceBuilderTests.Abc>().Should().NotBeNull();
            items.Application.GetService<ServiceBuilderTests.Abc2>().Should().NotBeNull();
            items.Application.GetService<ServiceBuilderTests.Abc3>().Should().BeNull();
            items.Application.GetService<ServiceBuilderTests.Abc4>().Should().BeNull();
        }

        [Fact]
        public void SendsNotificationThrough_OnBuild_Observable()
        {
            var assemblyProvider = new TestAssemblyProvider();
            var assemblyCandidateFinder = A.Fake<IAssemblyCandidateFinder>();
            var configuration = A.Fake<IConfiguration>();
            var scanner = new AggregateConventionScanner(assemblyCandidateFinder);
            var serviceCollection = new ServiceCollection();
            var servicesBuilder = new ApplicationServicesBuilder(scanner, assemblyProvider, assemblyCandidateFinder, serviceCollection, configuration, A.Fake<IHostingEnvironment>());
            var observer = A.Fake<IObserver<IServiceProvider>>();
            var observerApplication = A.Fake<IObserver<IServiceProvider>>();
            var observerSystem = A.Fake<IObserver<IServiceProvider>>();
            servicesBuilder.OnBuild.Subscribe(observer);
            servicesBuilder.Application.OnBuild.Subscribe(observerApplication);
            servicesBuilder.System.OnBuild.Subscribe(observerSystem);

            A.CallTo(() => assemblyCandidateFinder.GetCandidateAssemblies(A<IEnumerable<string>>._))
                .Returns(assemblyProvider.GetAssemblies());

            var items = servicesBuilder.Build(A.Fake<ILogger>());

            A.CallTo(() => observer.OnNext(items.Application)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => observerApplication.OnNext(items.Application)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => observerSystem.OnNext(items.System)).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
