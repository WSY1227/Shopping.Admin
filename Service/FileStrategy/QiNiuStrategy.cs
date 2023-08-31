using Microsoft.AspNetCore.Http;
using Qiniu.Storage;
using Qiniu.Util;

namespace Service.FileStrategy
{
    public class QiNiuStrategy : Strategy
    {
        public override async Task<string> Upload(List<IFormFile> files)
        {
            var res = await Task.Run(() =>
            {
                var mac = new Mac("xxx", "xxx");
                var result = new List<string>();
                foreach (var formFile in files)
                {
                    if (formFile.Length <= 0) continue;
                    var filePath_temp = $"{AppContext.BaseDirectory}/Images_temp";
                    var fileName = $"{DateTime.Now:yyyyMMddHHmmssffff}{formFile.FileName}";
                    if (!Directory.Exists(filePath_temp))
                    {
                        Directory.CreateDirectory(filePath_temp);
                    }

                    using (var stream = System.IO.File.Create($"{filePath_temp}/{fileName}"))
                    {
                        formFile.CopyTo(stream);
                    }

                    // 上传文件名
                    var key = fileName;
                    // 本地文件路径
                    var filePath = $"{filePath_temp}/{fileName}";
                    // 存储空间名
                    var Bucket = "pl-static";
                    // 设置上传策略
                    var putPolicy = new PutPolicy();
                    // 设置要上传的目标空间
                    putPolicy.Scope = Bucket;
                    // 上传策略的过期时间(单位:秒)
                    //putPolicy.SetExpires(3600);
                    // 文件上传完毕后，在多少天后自动被删除
                    //putPolicy.DeleteAfterDays = 1;
                    // 生成上传token
                    var token = Auth.CreateUploadToken(mac, putPolicy.ToJsonString());
                    var config = new Config();
                    // 设置上传区域
                    config.Zone = Zone.ZONE_CN_East;
                    // 设置 http 或者 https 上传
                    config.UseHttps = true;
                    config.UseCdnDomains = true;
                    config.ChunkSize = ChunkUnit.U512K;
                    // 表单上传
                    var target = new FormUploader(config);
                    var httpResult = target.UploadFile(filePath, key, token, null);
                    result.Add(fileName);
                    //删除备份文件夹
                    Directory.Delete(filePath_temp, true);
                }
                return string.Join(",", result);
            });
            return res;
        }
    }
}