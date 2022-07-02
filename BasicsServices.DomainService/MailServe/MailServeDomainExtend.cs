using BasicsServices.EntityDto.MailServe;
using BasicsServices.IDomainService.MailServe;
using Peihui.Common.Base.DataCheck;
using Peihui.Common.Base.UnifiedResponse;
using Peihui.Common.ExceptionUtils.Entity;
using Peihui.Common.ExceptionUtils.Exceptions;
using Peihui.Common.JsonHelper;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace BasicsServices.DomainService.MailServe
{
    public partial class MailServeDomain : IMailServeDomain
    {
        /// <summary>
        /// 设置上下文
        /// </summary>
        /// <param name="mailSendDto"></param>
        /// <param name="userContext"></param>
        /// <returns></returns>
        private MailSendDto SetContext(MailSendDto mailSendDto, UserContext userContext)
        {
            if (string.IsNullOrWhiteSpace(mailSendDto.MailFromInfo.FromName))
            {
                mailSendDto.MailFromInfo.FromName = userContext.Name;
            }
            return mailSendDto;
        }
        /// <summary>
        /// 校验数据
        /// </summary>
        /// <param name="mailSendDto"></param>
        private void SendDataValidator(MailSendDto mailSendDto)
        {
            if (string.IsNullOrWhiteSpace(mailSendDto.MailFromInfo.From))
            {
                throw new CustomException(new ExceptionEntity(500, "发件人为空"));
            }

            if (mailSendDto.MailAddresseeInfo.ReceiveAddList == null)
            {
                throw new CustomException(new ExceptionEntity(500, "收件人为空"));
            }

            if (!DataRegExp.IsEmail(mailSendDto.MailFromInfo.From))
            {
                throw new CustomException(new ExceptionEntity(500, "邮件服务器地址为空或地址出错"));
            }
            if (mailSendDto.MailFromInfo.ServerType != MailServerTypeEnum.Smtp)
            {
                throw new CustomException(new ExceptionEntity(400, "只支持SMTP方式发送邮件"));
            }
        }



        #region  其他业务逻辑
        /// <summary>
        /// 设置邮件发送客户端
        /// </summary>
        /// <param name="mailSendDto"></param>
        /// <param name="mailMsg"></param>
        private void MailSendDemo(MailSendDto mailSendDto, MailMessage mailMsg)
        {
            //指定smtp服务地址（根据发件人邮箱指定对应SMTP服务器地址）
            SmtpClient client = new SmtpClient
            {
                Host = mailSendDto.MailFromInfo.Server,
                //要用587端口
                Port = Convert.ToInt32(JsonConfigHelper.Configuration["MailServe:Port"]),//端口
                                                                                             //加密
                EnableSsl = true,
                //通过用户名和密码验证发件人身份
                Credentials = new NetworkCredential(mailSendDto.MailFromInfo.From, mailSendDto.MailFromInfo.AccessCode)
            };
            // 当Host为空时加上默认值
            if (string.IsNullOrWhiteSpace(mailSendDto.MailFromInfo.Server))
            {
                client.Host = JsonConfigHelper.Configuration[string.Format("MailServe:Host")];
            }
            client.Send(mailMsg);
        }
        #endregion
    }
}
