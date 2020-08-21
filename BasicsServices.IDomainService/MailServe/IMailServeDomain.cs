using BasicsServices.EntityDto.MailServe;
using Peihui.Core.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicsServices.IDomainService.MailServe
{
    public interface IMailServeDomain
    {
        /// <summary>
        /// 发送一封邮件(测试)
        /// </summary>
        /// <param name="mailMessageInput"></param>
        /// <param name="userContext"></param>
        /// <returns></returns>
        ResponseResult SendAppoint(MailMessageInput mailMessageInput, UserContext userContext);
        /// <summary>
        /// 发送一封邮件
        /// </summary>
        /// <param name="mailSendDto"></param>
        /// <param name="userContext"></param>
        /// <returns></returns>
        ResponseResult SendOne(MailSendDto mailSendDto, UserContext userContext);
        /// <summary>
        /// 批量发送
        /// </summary>
        /// <param name="mailSendDtos"></param>
        /// <param name="userContext"></param>
        /// <returns></returns>
        ResponseResult BatchSend(List<MailSendDto> mailSendDtos, UserContext userContext);
    }
}
