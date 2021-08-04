using System.Collections.Generic;
using NzbDrone.Common.Composition;
using NzbDrone.Common.EnvironmentInfo;
using NzbDrone.SignalR;

namespace NzbDrone.Host
{
    public class MainAppContainerBuilder : ContainerBuilderBase
    {
        public static IContainer BuildContainer(StartupContext args)
        {
            var assemblies = new List<string>
                             {
                                 "Lidarr.Host",
                                 "Lidarr.Core",
                                 "Lidarr.SignalR",
                                 "Lidarr.Api.V1",
                                 "Lidarr.Http"
                             };

            return new MainAppContainerBuilder(args, assemblies).Container;
        }

        private MainAppContainerBuilder(StartupContext args, List<string> assemblies)
            : base(args, assemblies)
        {
            AutoRegisterImplementations<MessageHub>();

            if (OsInfo.IsWindows)
            {
                Container.Register<INzbDroneServiceFactory, NzbDroneServiceFactory>();
            }
            else
            {
                Container.Register<INzbDroneServiceFactory, DummyNzbDroneServiceFactory>();
            }
        }
    }
}
