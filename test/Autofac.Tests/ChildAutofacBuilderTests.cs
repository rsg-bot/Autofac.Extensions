using System;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Extensions.DependencyInjection;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.Hosting;
using Xunit;
using Xunit.Abstractions;

namespace Rocket.Surgery.Extensions.Autofac.Tests
{
    public class ChildAutofacBuilderTests : AutoTestBase
    {
        public ChildAutofacBuilderTests(ITestOutputHelper outputHelper) : base(outputHelper) { }

        class ChildAutofacBuilder : Autofac.ChildAutofacBuilder
        {
            public ChildAutofacBuilder(IAutofacBuilder parent) : base(parent)
            {
            }
        }

        [Fact]
        public void Constructs()
        {
            var assemblytProvider = AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
            var services = AutoFake.Provide<IServiceCollection>(new ServiceCollection());
            var servicesBuilder = new ChildAutofacBuilder(AutoFake.Resolve<AutofacBuilder>());

            servicesBuilder.AssemblyProvider.Should().BeSameAs(assemblytProvider);
            servicesBuilder.AssemblyCandidateFinder.Should().NotBeNull();
            servicesBuilder.Services.Should().BeSameAs(services);
            servicesBuilder.Configuration.Should().NotBeNull();
            servicesBuilder.Application.Should().NotBeNull();
            servicesBuilder.System.Should().NotBeNull();
            servicesBuilder.Environment.Should().NotBeNull();

            Action a = () => { servicesBuilder.AddConvention(A.Fake<IAutofacConvention>()); };
            a.Should().NotThrow();
            a = () => { servicesBuilder.AddDelegate(delegate {  }); };
            a.Should().NotThrow();
            a = () => { servicesBuilder.ConfigureContainer(delegate {  }); };
            a.Should().NotThrow();
        }

        [Fact]
        public void StoresAndReturnsItems()
        {
            var servicesBuilder = new ChildAutofacBuilder(AutoFake.Resolve<AutofacBuilder>());

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
            var servicesBuilder = new ChildAutofacBuilder(new AutofacBuilder(scanner, assemblyProvider, assemblyCandidateFinder, serviceCollection, configuration, A.Fake<IHostingEnvironment>()));

            servicesBuilder[string.Empty].Should().BeNull();
        }

        [Fact]
        public void Nests()
        {
            var parent = AutoFake.Resolve<AutofacBuilder>();
            var servicesBuilder = new ChildAutofacBuilder(parent);

            servicesBuilder[string.Empty].Should().BeNull();

            servicesBuilder.Exit().Should().BeSameAs(parent);
        }
    }
}
