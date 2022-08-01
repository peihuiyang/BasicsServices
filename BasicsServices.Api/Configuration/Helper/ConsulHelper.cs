using Consul;
using Microsoft.Extensions.Configuration;
using Peihui.Common.JsonHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasicsServices.Api.Configuration.Helper
{
    /// <summary>
    /// Consul帮助类
    /// </summary>
    public static class ConsulHelper
    {
        /// <summary>
        /// Consul注册
        /// </summary>
        /// <param name="configuration"></param>
        public static void ConsulRegister(this IConfiguration configuration)
        {
            if (!bool.Parse(JsonConfigHelper.GetSectionValue("ConsulConfig:IsActive")))
                return;
            string consulAddress = configuration["consul"];
            string ipAddress = configuration["apiadd"];
            int port = int.Parse(configuration["apiport"]);

            //string consulAddress = JsonConfigHelper.GetSectionValue("ConsulConfig:ServerUrl");
            //string ipAddress = JsonConfigHelper.GetSectionValue("ConsulConfig:ClientIp");
            //int port = int.Parse(JsonConfigHelper.GetSectionValue("ConsulConfig:ClientPort"));

            ConsulClient client = new(c =>
            {
                c.Address = new Uri(consulAddress);
                c.Datacenter = "dc1";
            });

            client.Agent.ServiceRegister(new AgentServiceRegistration()
            {
                // 服务标识
                ID = $"Api-{ipAddress}-{port}",
                // 服务分组【修改为对应】
                Name = "BasicsServicesApi",
                Address = ipAddress,
                Port = port,
                Tags = new string[] { "Inner" },
                Check = new AgentServiceCheck()
                {
                    // 每间隔12秒检测一次
                    Interval = TimeSpan.FromSeconds(12),
                    // 修改为对应的api地址
                    HTTP = $"http://{ipAddress}:{port}/api/basics/health/test",
                    // 5秒钟内没响应就超时，检测等待时间
                    Timeout = TimeSpan.FromSeconds(5),
                    // 60秒钟内还是不能成功就移除
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(60)
                }
            });
        }
    }
}
