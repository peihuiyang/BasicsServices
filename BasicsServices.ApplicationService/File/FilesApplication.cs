using BasicsServices.EntityDto.File;
using BasicsServices.IApplicationService.File;
using Peihui.Common.Base.UnifiedResponse;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BasicsServices.ApplicationService.File
{
    public partial class FilesApplication : IFilesApplication
    {
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="token"></param>
        /// <param name="objectid"></param>
        /// <returns></returns>
        public ResponseResult DeteleCache(string token, string objectid)
        {
            // =>校验登录
            UserContext userContext = _enableLoginApplication.GetUserContext(token);
            // =>校验动作操作权限
            this.DoCheckFunAuthority(userContext, "");
            return _filesDomain.DeteleCache(userContext, objectid);
        }

        public ResponseResult<FileDto> ReadCache(string token, string objectid)
        {
            // =>校验登录
            UserContext userContext = _enableLoginApplication.GetUserContext(token);
            // =>校验动作操作权限
            this.DoCheckFunAuthority(userContext, "");
            return _filesDomain.ReadCache(userContext, objectid);
        }

        public ResponseResult<FileDto> ReadImage(string token, string objectid)
        {
            // =>校验登录
            UserContext userContext = _enableLoginApplication.GetUserContext(token);
            // =>校验动作操作权限
            this.DoCheckFunAuthority(userContext, "");
            return _filesDomain.ReadImage(userContext, objectid);
        }

        public ResponseResult<FileDto> UploadCache(string token, string fileName, Stream stream)
        {
            // =>校验登录
            UserContext userContext = _enableLoginApplication.GetUserContext(token);
            // =>校验动作操作权限
            this.DoCheckFunAuthority(userContext, "");
            return _filesDomain.UploadCache(userContext, fileName, stream);
        }
    }
}
