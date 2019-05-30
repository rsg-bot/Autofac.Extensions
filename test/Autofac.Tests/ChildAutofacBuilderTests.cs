using System;
using System.Collections.Generic;
using System.Diagnostics;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Extensions.DependencyInjection;
using Rocket.Surgery.Extensions.Testing;
using Xunit;
using Xunit.Abstractions;

namespace Rocket.Surgery.Extensions.Autofac.Tests
{
    public class ChildAutofacBuilderTests : AutoTestBase
    {
        public ChildAutofacBuilderTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
            AutoFake.Provide<DiagnosticSource>(new DiagnosticListener("Test"));
        }

        class ChildAutofacBuilder : Autofac.ChildAutofacBuilder
        {
            public ChildAutofacBuilder(AutofacBuilder parent) : base(parent)
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
            servicesBuilder.Environment.Should().NotBeNull();
        }

        [Fact]
        public void StoresAndReturnsItems()
        {
            AutoFake.Provide<IDictionary<object, object>>(new Dictionary<object, object>());
            var servicesBuilder = new ChildAutofacBuilder(AutoFake.Resolve<AutofacBuilder>());

            var value = new object();
            servicesBuilder[string.Empty] = value;
            servicesBuilder[string.Empty].Should().BeSameAs(value);
        }

        [Fact]
        public void IgnoreNonExistentItems()
        {
            AutoFake.Provide<IDictionary<object, object>>(new Dictionary<object, object>());
            var servicesBuilder = new ChildAutofacBuilder(AutoFake.Resolve<AutofacBuilder>());

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
