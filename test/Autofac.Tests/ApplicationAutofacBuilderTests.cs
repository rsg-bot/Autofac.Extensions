using System;
using System.Collections.Generic;
using Autofac;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Extensions.DependencyInjection;
using Rocket.Surgery.Extensions.Testing;
using Xunit;
using Xunit.Abstractions;

namespace Rocket.Surgery.Extensions.Autofac.Tests
{
    public class ApplicationAutofacBuilderTests : AutoTestBase
    {
        public ApplicationAutofacBuilderTests(ITestOutputHelper outputHelper) : base(outputHelper){}

        [Fact]
        public void Constructs()
        {
            var assemblytProvider = AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
            var services = AutoFake.Provide<IServiceCollection>(new ServiceCollection());
            var servicesBuilder = AutoFake.Resolve<ApplicationAutofacBuilder>();

            servicesBuilder.AssemblyProvider.Should().BeSameAs(assemblytProvider);
            servicesBuilder.AssemblyCandidateFinder.Should().NotBeNull();
            servicesBuilder.Services.Should().BeSameAs(services);
            servicesBuilder.Configuration.Should().NotBeNull();
            servicesBuilder.Application.Should().NotBeNull();
            servicesBuilder.System.Should().NotBeNull();
            servicesBuilder.Environment.Should().NotBeNull();

            Action a = () => { servicesBuilder.PrependConvention(A.Fake<IAutofacConvention>()); };
            a.Should().NotThrow();
            a = () => { servicesBuilder.PrependDelegate(delegate { }); };
            a.Should().NotThrow();
            a = () => { servicesBuilder.ConfigureContainer(delegate { }); };
            a.Should().NotThrow();
        }

        [Fact]
        public void StoresAndReturnsItems()
        {
            var servicesBuilder = AutoFake.Resolve<ApplicationAutofacBuilder>();

            var value = new object();
            servicesBuilder[string.Empty] = value;
            servicesBuilder[string.Empty].Should().BeSameAs(value);
        }

        [Fact]
        public void IgnoreNonExistentItems()
        {
            var servicesBuilder = AutoFake.Resolve<ApplicationAutofacBuilder>();

            servicesBuilder[string.Empty].Should().BeNull();
        }

        [Fact]
        public void AddConventions()
        {
            AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
            AutoFake.Provide<IServiceCollection>(new ServiceCollection());
            var servicesBuilder = AutoFake.Resolve<ApplicationAutofacBuilder>();

            var convention = A.Fake<IAutofacConvention>();

            servicesBuilder.PrependConvention(convention);

            A.CallTo(() => AutoFake.Resolve<IConventionScanner>().PrependConvention(convention)).MustHaveHappened();
        }

        [Fact]
        public void ConstructTheContainerAndRegisterWithCore()
        {

            AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
            AutoFake.Provide<IServiceCollection>(new ServiceCollection());
            var servicesBuilder = AutoFake.Resolve<ApplicationAutofacBuilder>();

            servicesBuilder.ConfigureContainer(c => c.RegisterInstance(A.Fake<AutofacBuilderTests.IAbc>()));
            servicesBuilder.Services.AddSingleton(A.Fake<AutofacBuilderTests.IAbc2>());
            servicesBuilder.System.ConfigureContainer(c => c.RegisterInstance(A.Fake<AutofacBuilderTests.IAbc3>()));

            var items = servicesBuilder.Build(new ContainerBuilder());
            items.Container.ResolveOptional<AutofacBuilderTests.IAbc>().Should().NotBeNull();
            items.Container.ResolveOptional<AutofacBuilderTests.IAbc2>().Should().NotBeNull();
            items.Container.ResolveOptional<AutofacBuilderTests.IAbc3>().Should().BeNull();
            items.Container.ResolveOptional<AutofacBuilderTests.IAbc4>().Should().BeNull();
        }

        [Fact]
        public void ConstructTheContainerAndRegisterWithApplication()
        {
            AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
            AutoFake.Provide<IServiceCollection>(new ServiceCollection());
            var servicesBuilder = AutoFake.Resolve<ApplicationAutofacBuilder>();

            servicesBuilder.Application.ConfigureContainer(c => c.RegisterInstance(A.Fake<AutofacBuilderTests.IAbc>()));
            servicesBuilder.Application.Services.AddSingleton(A.Fake<AutofacBuilderTests.IAbc2>());
            servicesBuilder.System.ConfigureContainer(c => c.RegisterInstance(A.Fake<AutofacBuilderTests.IAbc3>()));
            servicesBuilder.ConfigureContainer(c => c.RegisterInstance(A.Fake<AutofacBuilderTests.IAbc4>()));

            var items = servicesBuilder.Build(new ContainerBuilder());
            items.Application.ResolveOptional<AutofacBuilderTests.IAbc>().Should().NotBeNull();
            items.Application.ResolveOptional<AutofacBuilderTests.IAbc2>().Should().NotBeNull();
            items.Application.ResolveOptional<AutofacBuilderTests.IAbc3>().Should().BeNull();
            items.Application.ResolveOptional<AutofacBuilderTests.IAbc4>().Should().NotBeNull();
        }

        [Fact]
        public void ConstructTheContainerAndRegisterWithSystem()
        {
            AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
            AutoFake.Provide<IServiceCollection>(new ServiceCollection());
            var servicesBuilder = AutoFake.Resolve<ApplicationAutofacBuilder>();

            servicesBuilder.System.ConfigureContainer(c => c.RegisterInstance(A.Fake<AutofacBuilderTests.IAbc>()));
            servicesBuilder.System.Services.AddSingleton(A.Fake<AutofacBuilderTests.IAbc2>());
            servicesBuilder.Application.ConfigureContainer(c => c.RegisterInstance(A.Fake<AutofacBuilderTests.IAbc3>()));
            servicesBuilder.ConfigureContainer(c => c.RegisterInstance(A.Fake<AutofacBuilderTests.IAbc4>()));

            var items = servicesBuilder.Build(new ContainerBuilder());
            items.System.ResolveOptional<AutofacBuilderTests.IAbc>().Should().NotBeNull();
            items.System.ResolveOptional<AutofacBuilderTests.IAbc2>().Should().NotBeNull();
            items.System.ResolveOptional<AutofacBuilderTests.IAbc3>().Should().BeNull();
            items.System.ResolveOptional<AutofacBuilderTests.IAbc4>().Should().NotBeNull();
        }

        [Fact]
        public void ConstructTheContainerAndRegisterWithCore_ServiceProvider()
        {
            AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
            AutoFake.Provide<IServiceCollection>(new ServiceCollection());
            var servicesBuilder = AutoFake.Resolve<ApplicationAutofacBuilder>();

            servicesBuilder.ConfigureContainer(c => c.RegisterInstance(A.Fake<AutofacBuilderTests.IAbc>()));
            servicesBuilder.Services.AddSingleton(A.Fake<AutofacBuilderTests.IAbc2>());
            servicesBuilder.System.ConfigureContainer(c => c.RegisterInstance(A.Fake<AutofacBuilderTests.IAbc3>()));

            var items = servicesBuilder.Build(new ContainerBuilder());

            var sp = items.Container.Resolve<IServiceProvider>();
            sp.GetService<AutofacBuilderTests.IAbc>().Should().NotBeNull();
            sp.GetService<AutofacBuilderTests.IAbc2>().Should().NotBeNull();
            sp.GetService<AutofacBuilderTests.IAbc3>().Should().BeNull();
            sp.GetService<AutofacBuilderTests.IAbc4>().Should().BeNull();
        }

        [Fact]
        public void ConstructTheContainerAndRegisterWithApplication_ServiceProvider()
        {
            AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
            AutoFake.Provide<IServiceCollection>(new ServiceCollection());
            var servicesBuilder = AutoFake.Resolve<ApplicationAutofacBuilder>();

            servicesBuilder.Application.ConfigureContainer(c => c.RegisterInstance(A.Fake<AutofacBuilderTests.IAbc>()));
            servicesBuilder.Application.Services.AddSingleton(A.Fake<AutofacBuilderTests.IAbc2>());
            servicesBuilder.System.ConfigureContainer(c => c.RegisterInstance(A.Fake<AutofacBuilderTests.IAbc3>()));
            servicesBuilder.ConfigureContainer(c => c.RegisterInstance(A.Fake<AutofacBuilderTests.IAbc4>()));

            var items = servicesBuilder.Build(new ContainerBuilder());
            var sp = items.Application.Resolve<IServiceProvider>();
            sp.GetService<AutofacBuilderTests.IAbc>().Should().NotBeNull();
            sp.GetService<AutofacBuilderTests.IAbc2>().Should().NotBeNull();
            sp.GetService<AutofacBuilderTests.IAbc3>().Should().BeNull();
            sp.GetService<AutofacBuilderTests.IAbc4>().Should().NotBeNull();
        }

        [Fact]
        public void ConstructTheContainerAndRegisterWithSystem_ServiceProvider()
        {
            AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
            AutoFake.Provide<IServiceCollection>(new ServiceCollection());
            var servicesBuilder = AutoFake.Resolve<ApplicationAutofacBuilder>();

            servicesBuilder.System.ConfigureContainer(c => c.RegisterInstance(A.Fake<AutofacBuilderTests.IAbc>()));
            servicesBuilder.System.Services.AddSingleton(A.Fake<AutofacBuilderTests.IAbc2>());
            servicesBuilder.Application.ConfigureContainer(c => c.RegisterInstance(A.Fake<AutofacBuilderTests.IAbc3>()));
            servicesBuilder.ConfigureContainer(c => c.RegisterInstance(A.Fake<AutofacBuilderTests.IAbc4>()));

            var items = servicesBuilder.Build(new ContainerBuilder());
            var sp = items.System.Resolve<IServiceProvider>();
            sp.GetService<AutofacBuilderTests.IAbc>().Should().NotBeNull();
            sp.GetService<AutofacBuilderTests.IAbc2>().Should().NotBeNull();
            sp.GetService<AutofacBuilderTests.IAbc3>().Should().BeNull();
            sp.GetService<AutofacBuilderTests.IAbc4>().Should().NotBeNull();
        }

        [Fact]
        public void ConstructTheContainerAndRegisterWithSystem_UsingConvention()
        {
            var assemblyProvider = AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
            AutoFake.Provide<IConventionScanner>(AutoFake.Resolve<AggregateConventionScanner>());
            AutoFake.Provide<IServiceCollection>(new ServiceCollection());
            var servicesBuilder = AutoFake.Resolve<ApplicationAutofacBuilder>();

            A.CallTo(() => AutoFake.Resolve<IAssemblyCandidateFinder>().GetCandidateAssemblies(A<IEnumerable<string>>._))
                .Returns(assemblyProvider.GetAssemblies());

            var items = servicesBuilder.Build(new ContainerBuilder());
            items.Container.ResolveOptional<AutofacBuilderTests.IAbc>().Should().NotBeNull();
            items.Container.ResolveOptional<AutofacBuilderTests.IAbc2>().Should().NotBeNull();
            items.Container.ResolveOptional<AutofacBuilderTests.IAbc3>().Should().BeNull();
            items.Container.ResolveOptional<AutofacBuilderTests.IAbc4>().Should().BeNull();
        }

        [Fact]
        public void ConstructTheContainerAndRegisterWithSystem_UsingConvention_IncludingOtherBits()
        {
            var assemblyProvider = AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
            AutoFake.Provide<IConventionScanner>(AutoFake.Resolve<AggregateConventionScanner>());
            AutoFake.Provide<IServiceCollection>(new ServiceCollection());
            var servicesBuilder = AutoFake.Resolve<ApplicationAutofacBuilder>();

            A.CallTo(() => AutoFake.Resolve<IAssemblyCandidateFinder>().GetCandidateAssemblies(A<IEnumerable<string>>._))
                .Returns(assemblyProvider.GetAssemblies());

            var items = servicesBuilder.Build(new ContainerBuilder());
            items.Container.ResolveOptional<AutofacBuilderTests.IAbc>().Should().NotBeNull();
            items.Container.ResolveOptional<AutofacBuilderTests.IAbc2>().Should().NotBeNull();
            items.Container.ResolveOptional<AutofacBuilderTests.IAbc3>().Should().BeNull();
            items.Container.ResolveOptional<AutofacBuilderTests.IAbc4>().Should().BeNull();
            items.Container.ResolveOptional<AutofacBuilderTests.IOtherAbc3>().Should().NotBeNull();
            items.Container.ResolveOptional<AutofacBuilderTests.IOtherAbc3>().Should().NotBeNull();
        }

        [Fact]
        public void SendsNotificationThrough_OnBuild_Observable_ForMicrosoftExtensions()
        {
            var assemblyProvider = AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
            AutoFake.Provide<IServiceCollection>(new ServiceCollection());
            var servicesBuilder = AutoFake.Resolve<ApplicationAutofacBuilder>();

            A.CallTo(() => AutoFake.Resolve<IAssemblyCandidateFinder>().GetCandidateAssemblies(A<IEnumerable<string>>._))
                .Returns(assemblyProvider.GetAssemblies());

            var observer = A.Fake<IObserver<IServiceProvider>>();
            var observerApplication = A.Fake<IObserver<IServiceProvider>>();
            var observerSystem = A.Fake<IObserver<IServiceProvider>>();
            ((IServiceConventionContext)servicesBuilder).OnBuild.Subscribe(observer);
            ((IServiceConventionContext)servicesBuilder).Application.OnBuild.Subscribe(observerApplication);
            ((IServiceConventionContext)servicesBuilder).OnBuild.Subscribe(observerSystem);

            var items = servicesBuilder.Build(new ContainerBuilder());

            A.CallTo(() => observer.OnNext(A<IServiceProvider>.Ignored)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => observerApplication.OnNext(A<IServiceProvider>.Ignored)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => observerSystem.OnNext(A<IServiceProvider>.Ignored)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void SendsNotificationThrough_OnBuild_Observable_ForAutofac()
        {
            var assemblyProvider = AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
            AutoFake.Provide<IServiceCollection>(new ServiceCollection());
            var servicesBuilder = AutoFake.Resolve<ApplicationAutofacBuilder>();

            A.CallTo(() => AutoFake.Resolve<IAssemblyCandidateFinder>().GetCandidateAssemblies(A<IEnumerable<string>>._))
                .Returns(assemblyProvider.GetAssemblies());

            var observer = A.Fake<IObserver<ILifetimeScope>>();
            var observerContainer = A.Fake<IObserver<IContainer>>();
            var observerApplication = A.Fake<IObserver<ILifetimeScope>>();
            var observerSystem = A.Fake<IObserver<ILifetimeScope>>();
            servicesBuilder.OnContainerBuild.Subscribe(observerContainer);
            servicesBuilder.OnBuild.Subscribe(observer);
            servicesBuilder.Application.OnBuild.Subscribe(observerApplication);
            servicesBuilder.System.OnBuild.Subscribe(observerSystem);

            var items = servicesBuilder.Build(new ContainerBuilder());

            A.CallTo(() => observer.OnNext(items.Container)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => observerApplication.OnNext(items.Application)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => observerSystem.OnNext(items.System)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => observerContainer.OnNext(items.Container)).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
