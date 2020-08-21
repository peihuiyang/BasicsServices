using BasicsServices.IRepository.File;
using MongoDB.Bson;
using MongoDB.Driver.GridFS;
using Peihui.Common.Cache.MongoDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BasicsServices.Repository.File
{
    public class FilesRepository : IFilesRepository
    {
        private readonly IGridFSHelper _gridFSHelper;
        public FilesRepository(IGridFSHelper gridFSHelper)
        {
            _gridFSHelper = gridFSHelper;
        }

        public Task DeteleCache(string objectid)
        {
            return _gridFSHelper.DeleteFile(objectid);
        }

        public byte[] GetFileBytes(string objectid)
        {
            return _gridFSHelper.GetFileBytes(objectid);
        }

        public GridFSFileInfo GetFileCache(string objectid)
        {
            return _gridFSHelper.GetFileById(objectid);
        }

        public ObjectId UploadCache(string fileName, Stream stream)
        {
            return _gridFSHelper.UploadFile(fileName, stream);
        }
    }
}
