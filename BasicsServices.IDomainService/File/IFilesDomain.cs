using BasicsServices.EntityDto.File;
using Peihui.Core.Response;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BasicsServices.IDomainService.File
{
    /// <summary>
    /// 文件业务接口
    /// </summary>
    public interface IFilesDomain
    {
        /// <summary>
        /// 上传文件到缓存
        /// </summary>
        /// <param name="userContext"></param>
        /// <param name="fileName"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        ResponseResult<FileDto> UploadCache(UserContext userContext, string fileName, Stream stream);
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="userContext"></param>
        /// <param name="objectid"></param>
        /// <returns></returns>
        ResponseResult DeteleCache(UserContext userContext, string objectid);
        ResponseResult<FileDto> ReadCache(UserContext userContext, string objectid);
        ResponseResult<FileDto> ReadImage(UserContext userContext, string objectid);
    }
}
