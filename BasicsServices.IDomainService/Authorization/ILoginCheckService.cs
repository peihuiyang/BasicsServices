using Peihui.Common.Base.UnifiedResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicsServices.IDomainService.Authorization
{
    /// <summary>
    /// 登录校验接口
    /// </summary>
    public interface ILoginCheckService
    {
        /// <summary>
        /// 登录校验
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        UserContext LoginCheck(string token);
    }
}
