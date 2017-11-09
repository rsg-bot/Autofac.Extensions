using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;

namespace Rocket.Surgery.Extensions.Autofac
{
    public interface IServiceConventionContext : IConventionContext, IServiceConventionItem
    {
        IConfiguration Configuration { get; }
        IHostingEnvironment HostingEnvironment { get; }
        IAssemblyProvider AssemblyProvider { get; }
        IAssemblyCandidateFinder AssemblyCandidateFinder { get; }
        IServiceConventionItem System { get; }
        IServiceConventionItem Application { get; }
    }
}
