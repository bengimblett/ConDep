﻿using System;
using ConDep.Dsl.Builders;
using ConDep.Dsl.Operations.ApplicationRequestRouting;
using ConDep.Dsl.Operations.ApplicationRequestRouting.Infrastructure;

namespace ConDep.Dsl
{
	public static class ApplicationRequestRoutingExtension
	{
        public static ArrLoadBalancerOptions ApplicationRequestRouting(this LoadBalancerOptions loadBalancerOptions, string webServerName)//, Action<ApplicationRequestRoutingOptions> options)
		{
			var arrOperation = new ApplicationReqeustRoutingOperation(webServerName);
			loadBalancerOptions.AddOperation(arrOperation);
            return new ArrLoadBalancerOptions(arrOperation);
			//options(new ApplicationRequestRoutingOptions(arrOperation));
		}

        public static ArrLoadBalancerOptions ApplicationRequestRouting(this LoadBalancerOptions loadBalancerOptions, string webServerName, UserInfo userInfo)
		{
			var arrOperation = new ApplicationReqeustRoutingOperation(webServerName, userInfo);
			loadBalancerOptions.AddOperation(arrOperation);
            return new ArrLoadBalancerOptions(arrOperation);
			//options(new ApplicationRequestRoutingOptions(arrOperation));
		}
	}
}