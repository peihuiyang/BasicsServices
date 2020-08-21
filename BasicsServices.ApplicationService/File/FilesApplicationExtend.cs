using BasicsServices.IApplicationService.ApplicationCommon;
using BasicsServices.IApplicationService.File;
using BasicsServices.IDomainService.File;
using Peihui.Core.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicsServices.ApplicationService.File
{
    public partial class FilesApplication : IFilesApplication
    {
        private readonly IFilesDomain _filesDomain;
        private readonly IEnableLoginApplication _enableLoginApplication;
        public FilesApplication(IFilesDomain filesDomain, IEnableLoginApplication enableLoginApplication)
        {
            _filesDomain = filesDomain;
            _enableLoginApplication = enableLoginApplication;
        }
        /// <summary>
        /// 校验动作操作权限
        /// 判断新增、修改、删除、查询、审核、弃审等动作执行是否有权限
        /// </summary>
        /// <param name="userContext"></param>
        /// <param name="funAuthorityId"></param>
        private void DoCheckFunAuthority(UserContext userContext, string funAuthorityId)
        {
            //if (EnableVerification.Verification)
            //{
            //    
            //}
        }
    }
}
