using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasicsServices.Api.Configuration.Swagger
{
    /// <summary>
    /// 控制器说明
    /// </summary>
    public class SwaggerControllerTag : IDocumentFilter
    {
        /// <summary>
        /// 应用
        /// </summary>
        /// <param name="swaggerDoc"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            swaggerDoc.Tags = new List<OpenApiTag>
            {
                //添加控制器类描述
                new OpenApiTag{ Name = "MailServe",Description = "邮件服务" },
                new OpenApiTag{ Name = "UploadFile",Description = "文件服务" },
            };
        }
    }
}
