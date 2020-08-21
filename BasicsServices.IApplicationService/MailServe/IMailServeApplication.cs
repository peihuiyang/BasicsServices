using BasicsServices.EntityDto.MailServe;
using Peihui.Core.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicsServices.IApplicationService.MailServe
{
    /// <summary>
    /// 邮件服务应用层接口
    /// </summary>
    public interface IMailServeApplication
    {
        /// <summary>
        /// 发送一封邮件
        /// </summary>
        /// <param name="mailMessageInput"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        ResponseResult SendAppoint(MailMessageInput mailMessageInput, string token);
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailSendDto"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        ResponseResult SendOne(MailSendDto mailSendDto, string token);
        /// <summary>
        /// 批量发送邮件
        /// </summary>
        /// <param name="mailSendDtos"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        ResponseResult BatchSend(List<MailSendDto> mailSendDtos, string token);
    }
}
