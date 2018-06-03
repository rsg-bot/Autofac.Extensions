using System;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Extensions.Testing;
using Xunit;
using Xunit.Abstractions;

namespace Rocket.Surgery.Extensions.DependencyInjection.Tests
{
    public class ChildServicesBuilderTests : AutoTestBase
    {
        public ChildServicesBuilderTests(ITestOutputHelper outputHelper) : base(outputHelper) { }

        class ChildBuilder : ChildServicesBuilder
        {
            public ChildBuilder(IServicesBuilder parent) : base(parent)
            {
            }
        }

        [Fact]
        public void Constructs()
        {
            var assemblytProvider = AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
            var services = AutoFake.Provide<IServiceCollection>(new ServiceCollection());
            var servicesBuilder = new ChildBuilder(AutoFake.Resolve<ServicesBuilder>());

            servicesBuilder.AssemblyProvider.Should().BeSameAs(assemblytProvider);
            servicesBuilder.AssemblyCandidateFinder.Should().NotBeNull();
            servicesBuilder.Services.Should().BeSameAs(services);
            servicesBuilder.Configuration.Should().NotBeNull();
            servicesBuilder.Application.Should().NotBeNull();
            servicesBuilder.System.Should().NotBeNull();
            servicesBuilder.Environment.Should().NotBeNull();
        }

        [Fact]
        public void StoresAndReturnsItems()
        {
            var servicesBuilder = new ChildBuilder(AutoFake.Resolve<ApplicationServicesBuilder>());

            var value = new object();
            servicesBuilder[string.Empty] = value;
            servicesBuilder[string.Empty].Should().BeSameAs(value);
        }

        [Fact]
        public void IgnoreNonExistentItems()
        {
            var servicesBuilder = new ChildBuilder(AutoFake.Resolve<ApplicationServicesBuilder>());

            servicesBuilder[string.Empty].Should().BeNull();
        }

        [Fact]
        public void Nests()
        {
            var parent = AutoFake.Resolve<ServicesBuilder>();
            var servicesBuilder = new ChildBuilder(parent);

            servicesBuilder[string.Empty].Should().BeNull();

            servicesBuilder.Exit().Should().BeSameAs(parent);
        }
    }
}
