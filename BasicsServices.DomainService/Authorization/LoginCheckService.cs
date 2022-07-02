using BasicsServices.IDomainService.Authorization;
using Peihui.Cache.MyRedisHelper;
using Peihui.Common.Base.UnifiedResponse;
using Peihui.Common.ExceptionUtils.Enums;
using Peihui.Common.ExceptionUtils.Exceptions;
using Peihui.Common.JsonHelper;
using Peihui.Core.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicsServices.DomainService.Authorization
{
    /// <summary>
    /// 登录校验
    /// </summary>
    public class LoginCheckService : ILoginCheckService
    {
        /// <summary>
        /// 登录校验
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public UserContext LoginCheck(string token)
        {
            UserContext userContext = new UserContext();
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new CustomException(ExceptionType.TOKEN_ISNOT_NULL);
            }
            else if (token.Substring(7) == JsonConfigHelper.Configuration["TokenList:MailToken"])
            {
                userContext.BizId = "5c54ddfe28324d69af1b9c27ed139e75";
                userContext.Code = "MailAssistant";
                userContext.Name = "邮箱小助手";
                userContext.Token = "Bearer " + JsonConfigHelper.Configuration["TokenList:MailToken"];
            }
            else if (token.Substring(7) == "5c54ddfepei24d69af1b9chuid139e75")
            {
                userContext.BizId = "5c54yang28324d69af1b9c27ed139e75";
                userContext.Code = "FileAssistant";
                userContext.Name = "文件服务";
                userContext.Token = "Bearer 5c54ddfepei24d69af1b9chuid139e75";
            }
            else
            {
                userContext = this.GetContextByToken(token);
                if (userContext == null)
                {
                    throw new CustomException(ExceptionType.TOKEN_ISNOT_FIND);
                }
            }
            return userContext;
        }

        private UserContext GetContextByToken(string token)
        {
            var csredis = RedisClientFactory.GetClient(ConnConfig.GetConnectionString(ConnConfig.PASSPORTCACHE));
            if (!csredis.KeyExists(token))
            {
                return null;
            }
            else
            {
                UserContext userContext = csredis.StrGet<UserContext>(token);
                // 设置过期时间
                csredis.KeyExpire(token, Convert.ToInt32(JsonConfigHelper.Configuration["BasicConfig:EXPIRE_TIME"]));
                csredis.KeyExpire(userContext.BizId, Convert.ToInt32(JsonConfigHelper.Configuration["BasicConfig:EXPIRE_TIME"]));
                return userContext;
            }
        }
    }
}
