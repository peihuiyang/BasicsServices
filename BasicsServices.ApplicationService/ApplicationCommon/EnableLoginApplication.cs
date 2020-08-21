using BasicsServices.IApplicationService.ApplicationCommon;
using Peihui.Common.Security.Authorization;
using Peihui.Core.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicsServices.ApplicationService.ApplicationCommon
{
    /// <summary>
    /// 登录校验封装实现类
    /// </summary>
    public class EnableLoginApplication : IEnableLoginApplication
    {
        /// <summary>
        /// 登录校验服务
        /// </summary>
        private readonly ILoginCheckService _loginCheckServer;
        /// <summary>
        /// 构造注入
        /// </summary>
        /// <param name="loginCheckServer"></param>
        public EnableLoginApplication(ILoginCheckService loginCheckServer)
        {
            _loginCheckServer = loginCheckServer;
        }
        /// <summary>
        /// 获取UserContext
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public UserContext GetUserContext(string token)
        {
            UserContext userContext;
            if (EnableVerification.Verification)
            {
                // =>校验登录
                userContext = _loginCheckServer.LoginCheck(token);

            }
            else
            {
                userContext = new UserContext()
                {
                    BizId = "13bff123d61b48b5aab2ffb727e0720e",
                    Code = "sadmin",
                    Name = "小杨",
                    Token = token
                };
            }
            return userContext;
        }
    }
}
