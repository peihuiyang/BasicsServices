using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Peihui.Common.Base.UnifiedResponse;
using Peihui.Common.EntityDto.Command.LogInfo;
using Peihui.Common.ExceptionUtils.Exceptions;
using Peihui.Common.RabbitMqHelper.Factory;
using Peihui.Common.RabbitMqHelper.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BasicsServices.Api.Filters
{
    /// <summary>
    /// 全局异常过滤器
    /// </summary>
    public class HttpGlobalExceptionFilter : IAsyncExceptionFilter
    {
        /// <summary>
        /// 日志
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// host主机
        /// </summary>
        private readonly IWebHostEnvironment _env;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="env"></param>
        /// <param name="logger"></param>
        public HttpGlobalExceptionFilter(IWebHostEnvironment env, ILoggerFactory logger)
        {
            _env = env;
            _logger = logger.CreateLogger<HttpGlobalExceptionFilter>();
        }

        public Task OnExceptionAsync(ExceptionContext context)
        {
            // =>开发环境
            if (!_env.IsDevelopment())
            {
                _logger.LogError($"\r\n全局异常处理程序捕获到异常：\r\n错误消息：{context.Exception.Message}\r\n错误堆栈：{context.Exception.StackTrace}");

                _logger.LogError(context.Exception.ToString());
            }
            // =>非开发环境
            else
            {
                if (context.Exception is CustomException)
                {
                    var ex = (CustomException)context.Exception;
                    var errInfo = ResponseResult.Default(ex.ErrorMsg,ex.StatusCode);
                    context.Result = new ObjectResult(errInfo) { StatusCode = (int)HttpStatusCode.OK };
                }
                // =>无权访问异常
                else if (context.Exception is UnauthorizedAccessException)
                {
                    context.Result = new ObjectResult(ResponseResult.Default("权限不足无法访问")) { StatusCode = (int)HttpStatusCode.Unauthorized };
                }
                else
                {
                    var ex = context.Exception;
                    context.Result = new ObjectResult(ResponseResult.Default(ex.Message)) { StatusCode = (int)HttpStatusCode.BadRequest };
                }
            }
            try
            {
                var mq = RabbitMQFactory.GetClient(MqOptionExtension.SetMqOptions());
                mq.PublishEvent(new ElExceptionsCommand()
                {
                    ExceptionCommands = new List<ElExceptionCommand>()
                    {
                        new ElExceptionCommand()
                        {
                            ProjectName = "BasicService",
                            ExceptionInfo = context.Exception,
                            LogLevelType = "Exception",
                            LogTags = new string[] { "异常处理" },
                            SourceName = nameof(HttpGlobalExceptionFilter)
                        }
                    }
                });

            }
            catch(Exception ex)
            {
                _logger.LogError($"记录日志失败，原因：{ex.Message}");
            }
            context.ExceptionHandled = true;
            return Task.CompletedTask;
        }
    }
}
