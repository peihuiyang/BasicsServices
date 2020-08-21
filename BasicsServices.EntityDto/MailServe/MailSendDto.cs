using System;
using System.Collections.Generic;
using System.Text;

namespace BasicsServices.EntityDto.MailServe
{
    /// <summary>
    /// 邮件发送实体
    /// </summary>
    public class MailSendDto
    {
        /// <summary>
        /// 发件人实体
        /// </summary>
        public FromInfo MailFromInfo { get; set; }
        /// <summary>
        /// 邮件内容
        /// </summary>
        public AddresseeInfo MailAddresseeInfo { get; set; }
    }
}
