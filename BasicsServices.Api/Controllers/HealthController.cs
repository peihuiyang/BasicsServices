using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasicsServices.Api.Controllers
{
    [Route("api/basics/health")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        /// <summary>
        /// 健康检查
        /// </summary>
        /// <returns></returns>
        [HttpGet("check")]
        public IActionResult Check()
        {
            return Ok();
        }

        /// <summary>
        /// 测试
        /// </summary>
        [HttpGet("test")]
        public void Test()
        {
            var ip = HttpContext.Connection.LocalIpAddress.ToString();
            Response.WriteAsync($"The IP is :【{ip}:{HttpContext.Connection.LocalPort}】, The CurTime is :{DateTime.Now:yyyy-MM-dd HH:mm:ss},From BasicsServicesApi_HealthController.");
        }
    }
}
