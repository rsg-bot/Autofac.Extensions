﻿namespace Rocket.Surgery.Extensions.DependencyInjection
{
    public class ServicesEnvironment : IServicesEnvironment
    {
        public ServicesEnvironment(string environmentName, string applicationName, string webRootPath, string contentRootPath)
        {
            EnvironmentName = environmentName;
            ApplicationName = applicationName;
            WebRootPath = webRootPath;
            ContentRootPath = contentRootPath;
        }

        public string EnvironmentName { get;  }
        public string ApplicationName { get; }
        public string WebRootPath { get;  }
        public string ContentRootPath { get; }
    }
}
