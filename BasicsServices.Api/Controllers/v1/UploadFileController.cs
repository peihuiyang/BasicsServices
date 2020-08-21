using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BasicsServices.EntityDto.File;
using BasicsServices.IApplicationService.File;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Peihui.Core.Response;

namespace BasicsServices.Api.Controllers.v1
{
    [Route("api/basics/v1/upload")]
    [ApiController]
    public class UploadFileController : ControllerBase
    {
        private readonly IFilesApplication _filesApplication;
        public UploadFileController(IFilesApplication filesApplication)
        {
            _filesApplication = filesApplication;
        }
        /// <summary>
        /// 获取图片
        /// </summary>
        /// <param name="objectid"></param>
        /// <returns></returns>
        [HttpGet("readimage")]
        public ResponseResult<FileDto> ReadImage(string objectid)
        {
            string token = Request.Headers["Authorization"].ToString();

            if (string.IsNullOrWhiteSpace(objectid))
            {
                return ResponseResult<FileDto>.Error("读取失败");
            }
            else
            {
                return _filesApplication.ReadImage(token, objectid);
            }

        }
        /// <summary>
        /// 上传文件到缓存
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("uploadcache")]
        public ResponseResult<FileDto> UploadCache([FromForm(Name = "file")] IFormFile file)
        {
            string token = Request.Headers["Authorization"].ToString();

            if (file == null)
            {
                return ResponseResult<FileDto>.Error("获取不到提交的文件");
            }
            else
            {
                return _filesApplication.UploadCache(token, file.FileName, file.OpenReadStream());
            }

        }
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="objectid">文件标识</param>
        /// <returns></returns>
        [HttpPost("deletecache")]
        public ResponseResult DeleteCache([FromBody] string objectid)
        {
            string token = Request.Headers["Authorization"].ToString();

            if (string.IsNullOrWhiteSpace(objectid))
            {
                return ResponseResult.Error("请传文件ID");
            }
            else
            {
                return _filesApplication.DeteleCache(token, objectid);
            }

        }
        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="objectid"></param>
        /// <returns></returns>
        [HttpPost("readcache")]
        public ResponseResult<FileDto> ReadCache([FromBody] string objectid)
        {
            string token = Request.Headers["Authorization"].ToString();

            if (string.IsNullOrWhiteSpace(objectid))
            {
                return ResponseResult<FileDto>.Error("读取失败");
            }
            else
            {
                return _filesApplication.ReadCache(token, objectid);
            }

        }
    }
}