using System;
using System.Collections.Generic;
using System.Text;

namespace BasicsServices.EntityDto.MailServe
{
    /// <summary>
    /// 发送人信息
    /// </summary>
    public class FromInfo
    {
        /// <summary>
        /// 发件地址
        /// </summary>
        public string From { get; set; }
        /// <summary>
        /// 发送人
        /// </summary>
        public string FromName { get; set; }
        /// <summary>
        /// 服务器
        /// </summary>
        public string Server { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// 校验码
        /// </summary>
        public string AccessCode { get; set; }

        /// <summary>
        /// 服务器类型
        /// </summary>
        public MailServerTypeEnum ServerType { get; set; }
    }
}
