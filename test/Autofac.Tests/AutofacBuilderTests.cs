using System;
using System.Collections.Generic;
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
using Rocket.Surgery.Extensions.DependencyInjection;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.Hosting;
using Xunit;
using Xunit.Abstractions;

[assembly: Convention(typeof(AutofacBuilderTests.AbcConvention))]
[assembly: Convention(typeof(AutofacBuilderTests.OtherConvention))]

namespace Rocket.Surgery.Extensions.Autofac.Tests
{
    public class AutofacBuilderTests : AutoTestBase
    {
        public AutofacBuilderTests(ITestOutputHelper outputHelper) : base(outputHelper){}

        [Fact]
        public void Constructs()
        {
            var assemblytProvider = AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
            var services = AutoFake.Provide<IServiceCollection>(new ServiceCollection());
            var servicesBuilder = AutoFake.Resolve<AutofacBuilder>();

            servicesBuilder.AssemblyProvider.Should().BeSameAs(assemblytProvider);
            servicesBuilder.AssemblyCandidateFinder.Should().NotBeNull();
            servicesBuilder.Services.Should().BeSameAs(services);
            servicesBuilder.Configuration.Should().NotBeNull();
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
            var servicesBuilder = AutoFake.Resolve<AutofacBuilder>();

            var value = new object();
            servicesBuilder[string.Empty] = value;
            servicesBuilder[string.Empty].Should().BeSameAs(value);
        }

        [Fact]
        public void IgnoreNonExistentItems()
        {
            var servicesBuilder = AutoFake.Resolve<AutofacBuilder>();

            servicesBuilder[string.Empty].Should().BeNull();
        }

        [Fact]
        public void AddConventions()
        {
            var servicesBuilder = AutoFake.Resolve<AutofacBuilder>();

            var convention = A.Fake<IAutofacConvention>();

            servicesBuilder.AddConvention(convention);

            A.CallTo(() => AutoFake.Resolve<IConventionScanner>().AddConvention(convention)).MustHaveHappened();
        }

        public interface IAbc { }
        public interface IAbc2 { }
        public interface IAbc3 { }
        public interface IAbc4 { }
        public interface IOtherAbc3 { }
        public interface IOtherAbc4 { }

        public class AbcConvention : IAutofacConvention
        {
            public void Register(IAutofacConventionContext context)
            {
                context.ConfigureContainer(c => c.RegisterInstance(A.Fake<IAbc>()));
                context.Services.AddSingleton(A.Fake<IAbc2>());
                context.System.ConfigureContainer(c => c.RegisterInstance(A.Fake<IAbc3>()));
                context.Application.ConfigureContainer(c => { });
            }
        }

        public class OtherConvention : IServiceConvention
        {
            public void Register(IServiceConventionContext context)
            {
                context.Services.AddSingleton(A.Fake<IOtherAbc3>());
                context.System.Services.AddSingleton(A.Fake<IOtherAbc4>());
            }
        }

        [Fact]
        public void ConstructTheContainerAndRegisterWithCore()
        {
            AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
            AutoFake.Provide<IServiceCollection>(new ServiceCollection());
            var servicesBuilder = AutoFake.Resolve<AutofacBuilder>();

            servicesBuilder.ConfigureContainer(c => c.RegisterInstance(A.Fake<IAbc>()));
            servicesBuilder.Services.AddSingleton(A.Fake<IAbc2>());
            servicesBuilder.System.ConfigureContainer(c => c.RegisterInstance(A.Fake<IAbc3>()));

            var items = servicesBuilder.Build(new ContainerBuilder());
            items.ResolveOptional<IAbc>().Should().NotBeNull();
            items.ResolveOptional<IAbc2>().Should().NotBeNull();
            items.ResolveOptional<IAbc3>().Should().BeNull();
            items.ResolveOptional<IAbc4>().Should().BeNull();
        }

        [Fact]
        public void ConstructTheContainerAndRegisterWithApplication()
        {
            AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
            AutoFake.Provide<IServiceCollection>(new ServiceCollection());
            var servicesBuilder = AutoFake.Resolve<AutofacBuilder>();

            servicesBuilder.Application.ConfigureContainer(c => c.RegisterInstance(A.Fake<IAbc>()));
            servicesBuilder.Application.Services.AddSingleton(A.Fake<IAbc2>());
            servicesBuilder.System.ConfigureContainer(c => c.RegisterInstance(A.Fake<IAbc3>()));
            servicesBuilder.ConfigureContainer(c => c.RegisterInstance(A.Fake<IAbc4>()));

            var items = servicesBuilder.Build(new ContainerBuilder());
            items.ResolveOptional<IAbc>().Should().NotBeNull();
            items.ResolveOptional<IAbc2>().Should().NotBeNull();
            items.ResolveOptional<IAbc3>().Should().BeNull();
            items.ResolveOptional<IAbc4>().Should().NotBeNull();
        }

        [Fact]
        public void ConstructTheContainerAndRegisterWithSystem()
        {
            AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
            AutoFake.Provide<IServiceCollection>(new ServiceCollection());
            var servicesBuilder = AutoFake.Resolve<AutofacBuilder>();

            servicesBuilder.System.ConfigureContainer(c => c.RegisterInstance(A.Fake<IAbc>()));
            servicesBuilder.System.Services.AddSingleton(A.Fake<IAbc2>());
            servicesBuilder.Application.ConfigureContainer(c => c.RegisterInstance(A.Fake<IAbc3>()));
            servicesBuilder.ConfigureContainer(c => c.RegisterInstance(A.Fake<IAbc4>()));

            var items = servicesBuilder.Build(new ContainerBuilder());
            items.ResolveOptional<IAbc>().Should().BeNull();
            items.ResolveOptional<IAbc2>().Should().BeNull();
            items.ResolveOptional<IAbc3>().Should().NotBeNull();
            items.ResolveOptional<IAbc4>().Should().NotBeNull();
        }

        [Fact]
        public void ConstructTheContainerAndRegisterWithCore_ServiceProvider()
        {
            AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
            AutoFake.Provide<IServiceCollection>(new ServiceCollection());
            var servicesBuilder = AutoFake.Resolve<AutofacBuilder>();

            servicesBuilder.ConfigureContainer(c => c.RegisterInstance(A.Fake<IAbc>()));
            servicesBuilder.Services.AddSingleton(A.Fake<IAbc2>());
            servicesBuilder.System.ConfigureContainer(c => c.RegisterInstance(A.Fake<IAbc3>()));

            var items = servicesBuilder.Build(new ContainerBuilder());

            var sp = items.Resolve<IServiceProvider>();
            sp.GetService<IAbc>().Should().NotBeNull();
            sp.GetService<IAbc2>().Should().NotBeNull();
            sp.GetService<IAbc3>().Should().BeNull();
            sp.GetService<IAbc4>().Should().BeNull();
        }

        [Fact]
        public void ConstructTheContainerAndRegisterWithApplication_ServiceProvider()
        {
            AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
            AutoFake.Provide<IServiceCollection>(new ServiceCollection());
            var servicesBuilder = AutoFake.Resolve<AutofacBuilder>();

            servicesBuilder.Application.ConfigureContainer(c => c.RegisterInstance(A.Fake<IAbc>()));
            servicesBuilder.Application.Services.AddSingleton(A.Fake<IAbc2>());
            servicesBuilder.System.ConfigureContainer(c => c.RegisterInstance(A.Fake<IAbc3>()));
            servicesBuilder.ConfigureContainer(c => c.RegisterInstance(A.Fake<IAbc4>()));

            var items = servicesBuilder.Build(new ContainerBuilder());
            var sp = items.Resolve<IServiceProvider>();
            sp.GetService<IAbc>().Should().NotBeNull();
            sp.GetService<IAbc2>().Should().NotBeNull();
            sp.GetService<IAbc3>().Should().BeNull();
            sp.GetService<IAbc4>().Should().NotBeNull();
        }

        [Fact]
        public void ConstructTheContainerAndRegisterWithSystem_ServiceProvider()
        {
            var servicesBuilder = AutoFake.Resolve<AutofacBuilder>();

            servicesBuilder.System.ConfigureContainer(c => c.RegisterInstance(A.Fake<IAbc>()));
            servicesBuilder.System.Services.AddSingleton(A.Fake<IAbc2>());
            servicesBuilder.Application.ConfigureContainer(c => c.RegisterInstance(A.Fake<IAbc3>()));
            servicesBuilder.ConfigureContainer(c => c.RegisterInstance(A.Fake<IAbc4>()));

            var items = servicesBuilder.Build(new ContainerBuilder());
            var sp = items.Resolve<IServiceProvider>();
            sp.GetService<IAbc>().Should().BeNull();
            sp.GetService<IAbc2>().Should().BeNull();
            sp.GetService<IAbc3>().Should().NotBeNull();
            sp.GetService<IAbc4>().Should().NotBeNull();
        }

        [Fact]
        public void ConstructTheContainerAndRegisterWithSystem_UsingConvention()
        {
            var assemblyProvider = AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
            AutoFake.Provide<IConventionScanner>(AutoFake.Resolve<AggregateConventionScanner>());
            AutoFake.Provide<IServiceCollection>(new ServiceCollection());
            var servicesBuilder = AutoFake.Resolve<AutofacBuilder>();

            A.CallTo(() => AutoFake.Resolve<IAssemblyCandidateFinder>().GetCandidateAssemblies(A<IEnumerable<string>>._))
                .Returns(assemblyProvider.GetAssemblies());

            var items = servicesBuilder.Build(new ContainerBuilder());
            items.ResolveOptional<IAbc>().Should().NotBeNull();
            items.ResolveOptional<IAbc2>().Should().NotBeNull();
            items.ResolveOptional<IAbc3>().Should().BeNull();
            items.ResolveOptional<IAbc4>().Should().BeNull();
        }

        [Fact]
        public void ConstructTheContainerAndRegisterWithSystem_UsingConvention_IncludingOtherBits()
        {
            var assemblyProvider = AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
            AutoFake.Provide<IConventionScanner>(AutoFake.Resolve<AggregateConventionScanner>());
            AutoFake.Provide<IServiceCollection>(new ServiceCollection());
            var servicesBuilder = AutoFake.Resolve<AutofacBuilder>();

            A.CallTo(() => AutoFake.Resolve<IAssemblyCandidateFinder>().GetCandidateAssemblies(A<IEnumerable<string>>._))
                .Returns(assemblyProvider.GetAssemblies());

            var items = servicesBuilder.Build(new ContainerBuilder());
            items.ResolveOptional<IAbc>().Should().NotBeNull();
            items.ResolveOptional<IAbc2>().Should().NotBeNull();
            items.ResolveOptional<IAbc3>().Should().BeNull();
            items.ResolveOptional<IAbc4>().Should().BeNull();
            items.ResolveOptional<IOtherAbc3>().Should().NotBeNull();
            items.ResolveOptional<IOtherAbc3>().Should().NotBeNull();
        }

        [Fact]
        public void SendsNotificationThrough_OnBuild_Observable_ForMicrosoftExtensions()
        {
            var assemblyProvider = AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
            AutoFake.Provide<IServiceCollection>(new ServiceCollection());
            var servicesBuilder = AutoFake.Resolve<AutofacBuilder>();

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
            var servicesBuilder = AutoFake.Resolve<AutofacBuilder>();

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

            var container = servicesBuilder.Build(new ContainerBuilder());

            A.CallTo(() => observer.OnNext(container)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => observerApplication.OnNext(container)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => observerSystem.OnNext(A<IContainer>._)).MustHaveHappened(Repeated.Never);
            A.CallTo(() => observerContainer.OnNext(container)).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
