using System;
using System.Collections.Generic;
using System.Diagnostics;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Extensions.Testing;
using Xunit;
using Xunit.Abstractions;

namespace Rocket.Surgery.Extensions.DependencyInjection.Tests
{
    public class ApplicationServiceBuilderTests : AutoTestBase
    {
        public ApplicationServiceBuilderTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
            AutoFake.Provide<DiagnosticSource>(new DiagnosticListener("Test"));
        }

        [Fact]
        public void Constructs()
        {
            var assemblyProvider = AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
            var services = AutoFake.Provide<IServiceCollection>(new ServiceCollection());
            var servicesBuilder = AutoFake.Resolve<ApplicationServicesBuilder>();

            servicesBuilder.AssemblyProvider.Should().BeSameAs(assemblyProvider);
            servicesBuilder.AssemblyCandidateFinder.Should().NotBeNull();
            servicesBuilder.Services.Should().BeSameAs(services);
            servicesBuilder.Configuration.Should().NotBeNull();
            servicesBuilder.Application.Should().NotBeNull();
            servicesBuilder.System.Should().NotBeNull();
            servicesBuilder.Environment.Should().NotBeNull();

            Action a = () => { servicesBuilder.PrependConvention(A.Fake<IServiceConvention>()); };
            a.Should().NotThrow();
            a = () => { servicesBuilder.PrependDelegate(delegate { }); };
            a.Should().NotThrow();
        }

        [Fact]
        public void StoresAndReturnsItems()
        {
            AutoFake.Provide<IDictionary<object, object>>(new Dictionary<object, object>());
            var servicesBuilder = AutoFake.Resolve<ApplicationServicesBuilder>();

            var value = new object();
            servicesBuilder[string.Empty] = value;
            servicesBuilder[string.Empty].Should().BeSameAs(value);
        }

        [Fact]
        public void IgnoreNonExistentItems()
        {
            AutoFake.Provide<IDictionary<object, object>>(new Dictionary<object, object>());
            var servicesBuilder = AutoFake.Resolve<ApplicationServicesBuilder>();

            servicesBuilder[string.Empty].Should().BeNull();
        }

        [Fact]
        public void AddConventions()
        {
            var servicesBuilder = AutoFake.Resolve<ApplicationServicesBuilder>();

            var convention = A.Fake<IServiceConvention>();

            servicesBuilder.PrependConvention(convention);

            A.CallTo(() => AutoFake.Resolve<IConventionScanner>().PrependConvention(convention)).MustHaveHappened();
        }

        [Fact]
        public void ConstructTheContainerAndRegisterWithCore_ServiceProvider()
        {
            AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
            AutoFake.Provide<IServiceCollection>(new ServiceCollection());
            var servicesBuilder = AutoFake.Resolve<ApplicationServicesBuilder>();

            servicesBuilder.Services.AddSingleton(A.Fake<ServiceBuilderTests.IAbc>());
            servicesBuilder.Services.AddSingleton(A.Fake<ServiceBuilderTests.IAbc2>());
            servicesBuilder.System.Services.AddSingleton(A.Fake<ServiceBuilderTests.IAbc3>());

            var items = servicesBuilder.Build();

            var sp = items.Application;
            sp.GetService<ServiceBuilderTests.IAbc>().Should().NotBeNull();
            sp.GetService<ServiceBuilderTests.IAbc2>().Should().NotBeNull();
            sp.GetService<ServiceBuilderTests.IAbc3>().Should().BeNull();
            sp.GetService<ServiceBuilderTests.IAbc4>().Should().BeNull();
        }

        [Fact]
        public void ConstructTheContainerAndRegisterWithApplication_ServiceProvider()
        {
            AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
            AutoFake.Provide<IServiceCollection>(new ServiceCollection());
            var servicesBuilder = AutoFake.Resolve<ApplicationServicesBuilder>();

            servicesBuilder.Application.Services.AddSingleton(A.Fake<ServiceBuilderTests.IAbc>());
            servicesBuilder.Application.Services.AddSingleton(A.Fake<ServiceBuilderTests.IAbc2>());
            servicesBuilder.System.Services.AddSingleton(A.Fake<ServiceBuilderTests.IAbc3>());
            servicesBuilder.Services.AddSingleton(A.Fake<ServiceBuilderTests.IAbc4>());

            var items = servicesBuilder.Build();
            var sp = items.Application;
            sp.GetService<ServiceBuilderTests.IAbc2>().Should().NotBeNull();
            sp.GetService<ServiceBuilderTests.IAbc3>().Should().BeNull();
            sp.GetService<ServiceBuilderTests.IAbc4>().Should().NotBeNull();
        }

        [Fact]
        public void ConstructTheContainerAndRegisterWithSystem_ServiceProvider()
        {
            AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
            AutoFake.Provide<IServiceCollection>(new ServiceCollection());
            var servicesBuilder = AutoFake.Resolve<ApplicationServicesBuilder>();

            servicesBuilder.System.Services.AddSingleton(A.Fake<ServiceBuilderTests.IAbc>());
            servicesBuilder.System.Services.AddSingleton(A.Fake<ServiceBuilderTests.IAbc2>());
            servicesBuilder.Application.Services.AddSingleton(A.Fake<ServiceBuilderTests.IAbc3>());
            servicesBuilder.Services.AddSingleton(A.Fake<ServiceBuilderTests.IAbc4>());

            var items = servicesBuilder.Build();
            var sp = items.System;
            sp.GetService<ServiceBuilderTests.IAbc>().Should().NotBeNull();
            sp.GetService<ServiceBuilderTests.IAbc2>().Should().NotBeNull();
            sp.GetService<ServiceBuilderTests.IAbc3>().Should().BeNull();
            sp.GetService<ServiceBuilderTests.IAbc4>().Should().NotBeNull();
        }

        [Fact]
        public void ConstructTheContainerAndRegisterWithSystem_UsingConvention()
        {
            var assemblyProvider = AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
            AutoFake.Provide<IConventionScanner>(AutoFake.Resolve<AggregateConventionScanner>());
            AutoFake.Provide<IServiceCollection>(new ServiceCollection());
            var servicesBuilder = AutoFake.Resolve<ApplicationServicesBuilder>();

            A.CallTo(() => AutoFake.Resolve<IAssemblyCandidateFinder>().GetCandidateAssemblies(A<IEnumerable<string>>._))
                .Returns(assemblyProvider.GetAssemblies());

            var items = servicesBuilder.Build();
            items.Application.GetService<ServiceBuilderTests.IAbc>().Should().NotBeNull();
            items.Application.GetService<ServiceBuilderTests.IAbc2>().Should().NotBeNull();
            items.Application.GetService<ServiceBuilderTests.IAbc3>().Should().BeNull();
            items.Application.GetService<ServiceBuilderTests.IAbc4>().Should().BeNull();
        }

        [Fact]
        public void SendsNotificationThrough_OnBuild_Observable()
        {
            var assemblyProvider = AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
            AutoFake.Provide<IConventionScanner>(AutoFake.Resolve<AggregateConventionScanner>());
            AutoFake.Provide<IServiceCollection>(new ServiceCollection());
            var servicesBuilder = AutoFake.Resolve<ApplicationServicesBuilder>();

            A.CallTo(() => AutoFake.Resolve<IAssemblyCandidateFinder>().GetCandidateAssemblies(A<IEnumerable<string>>._))
                .Returns(assemblyProvider.GetAssemblies());

            var observer = A.Fake<IObserver<IServiceProvider>>();
            var observerApplication = A.Fake<IObserver<IServiceProvider>>();
            var observerSystem = A.Fake<IObserver<IServiceProvider>>();
            servicesBuilder.OnBuild.Subscribe(observer);
            servicesBuilder.Application.OnBuild.Subscribe(observerApplication);
            servicesBuilder.System.OnBuild.Subscribe(observerSystem);

            var items = servicesBuilder.Build();

            A.CallTo(() => observer.OnNext(items.Application)).MustHaveHappenedOnceExactly();
            A.CallTo(() => observerApplication.OnNext(items.Application)).MustHaveHappenedOnceExactly();
            A.CallTo(() => observerSystem.OnNext(items.System)).MustHaveHappenedOnceExactly();
        }
    }
}
