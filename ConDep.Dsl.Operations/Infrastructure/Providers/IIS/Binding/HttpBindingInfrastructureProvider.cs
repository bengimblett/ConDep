﻿using System;
using ConDep.Dsl.Core;

namespace ConDep.Dsl.Providers.IIS.Binding
{
    public class HttpBindingInfrastructureProvider : WebDeployCompositeProvider
    {
        private readonly string _webSiteName;
        private readonly int _port;

        public HttpBindingInfrastructureProvider(string webSiteName, int port)
        {
            if(string.IsNullOrWhiteSpace(webSiteName)) throw new ArgumentNullException("webSiteName");

            _webSiteName = webSiteName;
            _port = port;
        }

        public HttpBindingInfrastructureProvider(string webSiteName, int port, Action<IisBindingInfrastructureOptions> bindingOptions)
        {
            _webSiteName = webSiteName;
            _port = port;
            throw new NotImplementedException();
        }

        public string HostHeader { get; set; }

        public string Ip { get; set; }

        public override bool IsValid(Notification notification)
        {
            throw new NotImplementedException();
        }

        public override void Configure(DeploymentServer server)
        {
            //Todo: get bindingtype
            var psCommand = CreateBinding(_webSiteName, Ip, HostHeader, _port.ToString(), BindingType.http);
            Configure(p => p.PowerShell("Import-Module WebAdministration; " + psCommand, o => o.WaitIntervalInSeconds(2).RetryAttempts(20)));
        }

        private static string CreateBinding(string webSiteName, string ip, string hostHeader, string port, BindingType bindingType)
        {
            var ipAddress = string.IsNullOrWhiteSpace(ip)
                                ? ""
                                : "-IPAddress \"" + ip + "\"";

            var hostHeader2 = string.IsNullOrWhiteSpace(hostHeader)
                                 ? ""
                                 : "-HostHeader \"" + hostHeader + "\"";

            return string.Format("New-WebBinding -Name \"{0}\" -Protocol \"{1}\" -Port {2} {3} {4} -force; ",
                              webSiteName, bindingType, port, ipAddress, hostHeader2);

        }
    }
}