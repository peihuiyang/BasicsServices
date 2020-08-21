using BasicsServices.IDomainService.File;
using BasicsServices.IRepository.File;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicsServices.DomainService.File
{
    /// <summary>
    /// 实现类
    /// </summary>
    public partial class FilesDomain : IFilesDomain
    {
        private readonly IFilesRepository _filesRepository;
        public FilesDomain(IFilesRepository filesRepository)
        {
            _filesRepository = filesRepository;
        }
    }
}
