using Peihui.Core.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicsServices.IApplicationService.ApplicationCommon
{
    /// <summary>
    /// 登录校验封装接口
    /// </summary>
    public interface IEnableLoginApplication
    {
        /// <summary>
        /// 获取UserContext
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        UserContext GetUserContext(string token);
    }
}
