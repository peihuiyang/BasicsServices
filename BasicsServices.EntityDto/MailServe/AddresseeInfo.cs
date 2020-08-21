using MimeKit;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicsServices.EntityDto.MailServe
{
    /// <summary>
    /// 文字邮件内容
    /// </summary>
    public class AddresseeInfo
    {
        /// <summary>
        /// 邮件主题
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 邮件内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 收件人地址列表
        /// 多个收件人用分号分隔
        /// </summary>
        public List<string> ReceiveAddList { get; set; }
        /// <summary>
        /// 附件列表
        /// </summary>
        public List<MimePart> AnnexList { get; set; }
    }
}
