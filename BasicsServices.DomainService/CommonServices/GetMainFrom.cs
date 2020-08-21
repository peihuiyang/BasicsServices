using BasicsServices.EntityDto.MailServe;
using Peihui.Code.DataCheck;
using Peihui.Core.Config;
using Peihui.Core.CustomException;
using Peihui.Core.EnDecrypt;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicsServices.DomainService.CommonServices
{
    /// <summary>
    /// 获取指定发件信息
    /// </summary>
    public class GetMainFrom
    {
        /// <summary>
        /// 获取发件地址
        /// </summary>
        /// <returns></returns>
        public static string GetFrom()
        {
            string from = JsonConfigHelper.Configuration[string.Format("MailServe:From")];
            if(string.IsNullOrWhiteSpace(from))
                throw new ExceptionHandle(new ExceptionEntity(400, "发件地址不能为空"));
            return from;
        }
        /// <summary>
        /// 获取发件人
        /// </summary>
        /// <returns></returns>
        public static string GetFromName()
        {
            string FromName = JsonConfigHelper.Configuration[string.Format("MailServe:FromName")];
            if (string.IsNullOrWhiteSpace(FromName))
                throw new ExceptionHandle(new ExceptionEntity(400, "发件人不能为空"));
            return FromName;
        }
        /// <summary>
        /// 获取校验码
        /// </summary>
        /// <returns></returns>
        public static string GetAccessCode()
        {
            string AccessCode = AesHelper.Decrypt(JsonConfigHelper.Configuration[string.Format("MailServe:AccessCode")]);
            if (string.IsNullOrWhiteSpace(AccessCode))
                throw new ExceptionHandle(new ExceptionEntity(400, "校验码不能为空"));
            return AccessCode;
        }
        /// <summary>
        /// 获取主机
        /// </summary>
        /// <returns></returns>
        public static string GetHost()
        {
            string Host = JsonConfigHelper.Configuration[string.Format("MailServe:Host")];
            if (string.IsNullOrWhiteSpace(Host))
                throw new ExceptionHandle(new ExceptionEntity(400, "服务器不能为空"));
            return Host;
        }
        /// <summary>
        /// 获取端口
        /// </summary>
        /// <returns></returns>
        public static int GetPort()
        {
            try
            {
                int Port = Convert.ToInt32(JsonConfigHelper.Configuration[string.Format("MailServe:Port")]);
                return Port;
            }
            catch
            {
                throw new ExceptionHandle(new ExceptionEntity(400, "校验码不能为空"));
            }                
        }
    }
}
