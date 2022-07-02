using BasicsServices.EntityDto.File;
using BasicsServices.IDomainService.File;
using Peihui.Common.Base.UnifiedResponse;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BasicsServices.DomainService.File
{
    /// <summary>
    /// 实现类
    /// </summary>
    public partial class FilesDomain : IFilesDomain
    {
        public ResponseResult DeteleCache(UserContext userContext, string objectid)
        {
            var fileInfo = _filesRepository.GetFileCache(objectid);
            if (fileInfo == null)
            {
                return ResponseResult.Error("找不到文件");
            }
            var result = _filesRepository.DeteleCache(objectid);
            if (result.IsFaulted)
            {
                return ResponseResult.Error("文件删除失败");
            }
            else
            {
                return ResponseResult.Success("文件删除成功");
            }
        }

        public ResponseResult<FileDto> ReadCache(UserContext userContext, string objectid)
        {
            FileDto fileDto = new FileDto();
            // 获取文件信息
            var fileInfo = _filesRepository.GetFileCache(objectid);
            // 获取文件字节数组
            var result = _filesRepository.GetFileBytes(objectid);
            // 转换为base64
            string base64 = Convert.ToBase64String(result);
            // => 包装实体
            fileDto.ObjectId = objectid;
            fileDto.FileName = fileInfo.Filename;
            fileDto.Base64String = base64;
            //// 把 byte[] 写入文件
            //FileStream fs = new FileStream(fileDto.FileName, FileMode.Create);
            //fileDto.FileUrl = fs.Name;
            //BinaryWriter bw = new BinaryWriter(fs);
            //bw.Write(result);
            //bw.Close();
            //fs.Close();
            return ResponseResult<FileDto>.Success(fileDto, "文件读取成功");
        }

        public ResponseResult<FileDto> ReadImage(UserContext userContext, string objectid)
        {
            FileDto fileDto = new FileDto();
            var fileInfo = _filesRepository.GetFileCache(objectid);
            var result = _filesRepository.GetFileBytes(objectid);
            fileDto.ObjectId = objectid;
            //fileDto.FileStream = result;
            fileDto.FileName = fileInfo.Filename;
            //获取文件后缀
            string fileNameEx = Path.GetExtension(fileDto.FileName);
            string base64 = Convert.ToBase64String(result);
            if (fileNameEx != ".png" && fileNameEx != ".jpg" && fileNameEx != ".jpeg")
            {
                return ResponseResult<FileDto>.Error("图片格式不正确");
            }
            else
            {
                fileDto.Base64String = "data:image/png;base64," + base64;
            }
            return ResponseResult<FileDto>.Success(fileDto, "图片读取成功");
        }

        /// <summary>
        /// 上传文件到缓存
        /// </summary>
        /// <param name="userContext"></param>
        /// <param name="fileName"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public ResponseResult<FileDto> UploadCache(UserContext userContext, string fileName, Stream stream)
        {
            fileName = Guid.NewGuid().ToString("N").ToLower() + Path.GetExtension(fileName);
            var objectid = _filesRepository.UploadCache(fileName, stream);
            FileDto fileDto = new FileDto()
            {
                ObjectId = objectid.ToString(),
                FileName = fileName
            };
            return ResponseResult<FileDto>.Success(fileDto, "文件上传成功");
        }
    }
}
