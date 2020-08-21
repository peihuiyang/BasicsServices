using BasicsServices.DomainService.CommonServices;
using BasicsServices.EntityDto.MailServe;
using BasicsServices.IDomainService.MailServe;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using Peihui.Code.DataCheck;
using Peihui.Core.CustomException;
using Peihui.Core.Response;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace BasicsServices.DomainService.MailServe
{
    /// <summary>
    /// 邮件服务业务实现
    /// </summary>
    public partial class MailServeDomain : IMailServeDomain
    {
        /// <summary>
        /// 发送一封邮件
        /// </summary>
        /// <param name="mailSendDto"></param>
        /// <param name="userContext"></param>
        /// <returns></returns>
        public ResponseResult SendOne(MailSendDto mailSendDto, UserContext userContext)
        {
            StringBuilder stringBuilder = new StringBuilder();
            mailSendDto = SetContext(mailSendDto, userContext);
            this.SendDataValidator(mailSendDto);
            #region 构建发件信息
            // 实例化对象
            MimeMessage mimeMessage = new MimeMessage();
            Multipart multipart = new Multipart("mixed");
            // 源邮件地址和发件人
            mimeMessage.From.Add(new MailboxAddress(mailSendDto.MailFromInfo.FromName, mailSendDto.MailFromInfo.From));
            // 添加收件人
            if (mailSendDto.MailAddresseeInfo.ReceiveAddList != null)
            {
                foreach (var item in mailSendDto.MailAddresseeInfo.ReceiveAddList)
                {
                    if (DataRegExp.IsEmail(item))
                    {
                        mimeMessage.To.Add(new MailboxAddress(item, item));//收件人地址
                    }
                    else
                    {
                        stringBuilder.Append(item + " ");
                    }
                }
            }
            else
            {
                throw new ExceptionHandle(new ExceptionEntity(400, "收件地址为空"));
            }
            //发送邮件的标题
            mimeMessage.Subject = mailSendDto.MailAddresseeInfo.Subject;
            // 邮件内容
            var plain = new TextPart(TextFormat.Html)
            {
                Text = mailSendDto.MailAddresseeInfo.Content
            };
            multipart.Add(plain);
            #region 添加邮件附件
            if (mailSendDto.MailAddresseeInfo.AnnexList != null)
            {
                foreach (var item in mailSendDto.MailAddresseeInfo.AnnexList)
                {
                    multipart.Add(item);
                }
            }
            #endregion
            //发送邮件的内容
            mimeMessage.Body = multipart;

            #endregion
            try
            {
                #region 执行发送
                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    client.Connect(mailSendDto.MailFromInfo.Server, mailSendDto.MailFromInfo.Port, SecureSocketOptions.Auto);
                    client.Authenticate(mailSendDto.MailFromInfo.From, mailSendDto.MailFromInfo.AccessCode);
                    client.Send(mimeMessage);
                    client.Disconnect(true);
                }
                #endregion
                return ResponseResult.Success("发送成功" + (stringBuilder.Length <= 0 ? "!" : ("。以下地址发送失败：" + stringBuilder)));
            }
            catch (Exception ex)
            {
                return ResponseResult.Error((stringBuilder.Length <= 0 ? "发送失败!" : ("以下地址发送失败：" + stringBuilder)) + "发送失败原因：" + ex);
            }
        }

        public ResponseResult BatchSend(List<MailSendDto> mailSendDtos, UserContext userContext)
        {
            throw new ExceptionHandle(new ExceptionEntity(400, "暂未实现该功能"));
        }
        /// <summary>
        /// 发送一封邮件（测试）
        /// </summary>
        /// <param name="mailMessageInput"></param>
        /// <param name="userContext"></param>
        /// <returns></returns>
        public ResponseResult SendAppoint(MailMessageInput mailMessageInput, UserContext userContext)
        {
            StringBuilder stringBuilder = new StringBuilder();
            #region 构建发件信息
            // 实例化对象
            MimeMessage mimeMessage = new MimeMessage();
            Multipart multipart = new Multipart("mixed");
            // 源邮件地址和发件人
            mimeMessage.From.Add(new MailboxAddress(GetMainFrom.GetFromName(), GetMainFrom.GetFrom()));
            // 添加收件人
            if (mailMessageInput.Addressees != null)
            {
                foreach (var item in mailMessageInput.Addressees)
                {
                    if (DataRegExp.IsEmail(item))
                    {
                        mimeMessage.To.Add(new MailboxAddress(item, item));//收件人地址
                    }
                    else
                    {
                        stringBuilder.Append(item + " ");
                    }
                }
            }
            else
            {
                throw new ExceptionHandle(new ExceptionEntity(400, "收件地址为空"));
            }
            //发送邮件的标题
            mimeMessage.Subject = mailMessageInput.Subject;
            // 邮件内容
            var plain = new TextPart(TextFormat.Html)
            {
                Text = mailMessageInput.MessageContent
            };
            multipart.Add(plain);
            //发送邮件的内容
            mimeMessage.Body = multipart;

            #endregion
            try
            {
                #region 执行发送
                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    client.Connect(GetMainFrom.GetHost(), GetMainFrom.GetPort(), SecureSocketOptions.Auto);
                    client.Authenticate(GetMainFrom.GetFrom(), GetMainFrom.GetAccessCode());
                    client.Send(mimeMessage);
                    client.Disconnect(true);
                }
                #endregion
                return ResponseResult.Success("发送成功" + (stringBuilder.Length <= 0 ? "!" : ("。以下地址发送失败：" + stringBuilder)));
            }
            catch (Exception ex)
            {
                return ResponseResult.Error((stringBuilder.Length <= 0 ? "发送失败!" : ("以下地址发送失败：" + stringBuilder)) + "发送失败原因：" + ex);
            }
        }
    }
}
