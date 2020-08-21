using MongoDB.Bson;
using MongoDB.Driver.GridFS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BasicsServices.IRepository.File
{
    /// <summary>
    /// 文件仓储接口
    /// </summary>
    public interface IFilesRepository
    {
        ObjectId UploadCache(string fileName, Stream stream);
        GridFSFileInfo GetFileCache(string objectid);
        Task DeteleCache(string objectid);
        byte[] GetFileBytes(string objectid);
    }
}
