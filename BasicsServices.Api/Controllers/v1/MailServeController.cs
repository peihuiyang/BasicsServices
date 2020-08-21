using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BasicsServices.EntityDto.MailServe;
using BasicsServices.IApplicationService.MailServe;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Peihui.Core.Response;

namespace BasicsServices.Api.Controllers.v1
{
    [Route("api/basics/v1/mailserve")]
    [ApiController]
    public class MailServeController : ControllerBase
    {
        private readonly IMailServeApplication _mailServeApplication;
        /// <summary>
        /// 依赖注入
        /// </summary>
        /// <param name="mailServeApplication"></param>
        public MailServeController(IMailServeApplication mailServeApplication)
        {
            _mailServeApplication = mailServeApplication;
        }
        /// <summary>
        /// 发送邮件(指定发件人，邮箱服务器, 不加附件)
        /// </summary>
        /// <param name="mailMessageInput">邮件内容</param>
        /// <returns></returns>
        [HttpPost("sendappoint")]
        public ResponseResult SendAppoint([FromBody] MailMessageInput mailMessageInput)
        {
            string token = Request.Headers["Authorization"].ToString();
            return _mailServeApplication.SendAppoint(mailMessageInput, token);
        }
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailSendDto"></param>
        /// <returns></returns>
        [HttpPost("sendone")]
        public ResponseResult SendOne([FromBody] MailSendDto mailSendDto)
        {
            string token = Request.Headers["Authorization"].ToString();
            return _mailServeApplication.SendOne(mailSendDto, token);
        }
        /// <summary>
        /// 批量发送邮件
        /// </summary>
        /// <param name="mailSendDtos"></param>
        /// <returns></returns>
        [HttpPost("batchsend")]
        public ResponseResult BatchSend([FromBody] List<MailSendDto> mailSendDtos)
        {
            string token = Request.Headers["Authorization"].ToString();
            return _mailServeApplication.BatchSend(mailSendDtos, token);
        }
    }
}