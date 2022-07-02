using BasicsServices.EntityDto.File;
using Peihui.Common.Base.UnifiedResponse;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BasicsServices.IApplicationService.File
{
    /// <summary>
    /// 文件应用接口
    /// </summary>
    public interface IFilesApplication
    {
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="token"></param>
        /// <param name="fileName"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        ResponseResult<FileDto> UploadCache(string token, string fileName, Stream stream);
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="token"></param>
        /// <param name="objectid"></param>
        /// <returns></returns>
        ResponseResult DeteleCache(string token, string objectid);
        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="token"></param>
        /// <param name="objectid"></param>
        /// <returns></returns>
        ResponseResult<FileDto> ReadCache(string token, string objectid);
        /// <summary>
        /// 获取图片
        /// </summary>
        /// <param name="token"></param>
        /// <param name="objectid"></param>
        /// <returns></returns>
        ResponseResult<FileDto> ReadImage(string token, string objectid);
    }
}
