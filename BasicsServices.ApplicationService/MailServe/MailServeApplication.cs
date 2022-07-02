using BasicsServices.EntityDto.MailServe;
using BasicsServices.IApplicationService.MailServe;
using Peihui.Common.Base.UnifiedResponse;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicsServices.ApplicationService.MailServe
{
    /// <summary>
    /// 邮件服务应用实现
    /// </summary>
    public partial class MailServeApplication : IMailServeApplication
    {

        /// <summary>
        /// 发送一封邮件
        /// </summary>
        /// <param name="mailSendDto"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public ResponseResult SendOne(MailSendDto mailSendDto, string token)
        {
            UserContext userContext = _enableLoginApplication.GetUserContext(token);

            return _mailServeDomain.SendOne(mailSendDto, userContext);
        }
        /// <summary>
        /// 批量发送
        /// </summary>
        /// <param name="mailSendDtos"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public ResponseResult BatchSend(List<MailSendDto> mailSendDtos, string token)
        {
            UserContext userContext = _enableLoginApplication.GetUserContext(token);

            return _mailServeDomain.BatchSend(mailSendDtos, userContext);
        }
        /// <summary>
        /// 发送一封邮件
        /// </summary>
        /// <param name="mailMessageInput"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public ResponseResult SendAppoint(MailMessageInput mailMessageInput, string token)
        {
            // =>校验登录
            UserContext userContext = _enableLoginApplication.GetUserContext(token);
            return _mailServeDomain.SendAppoint(mailMessageInput, userContext);
        }
    }
}
