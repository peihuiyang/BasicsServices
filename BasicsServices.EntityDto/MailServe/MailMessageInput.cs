using System;
using System.Collections.Generic;
using System.Text;

namespace BasicsServices.EntityDto.MailServe
{
    /// <summary>
    /// 邮件内容
    /// </summary>
    public class MailMessageInput
    {
        /// <summary>
        /// 收件人列表
        /// </summary>
        public List<string> Addressees { get; set; }
        /// <summary>
        /// 邮件标题
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// 邮件内容
        /// </summary>
        public string MessageContent { get; set; }
    }
}
