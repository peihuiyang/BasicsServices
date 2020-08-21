using System;
using System.Collections.Generic;
using System.Text;

namespace BasicsServices.EntityDto.File
{
    /// <summary>
    /// 文件传输实体
    /// </summary>
    public class FileDto
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ObjectId { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 文件路径
        /// </summary>
        public string FileUrl { get; set; }
        /// <summary>
        /// Base64
        /// </summary>
        public string Base64String { get; set; }
    }
}
