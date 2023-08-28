using Microsoft.AspNetCore.Http;

namespace Service.FileStrategy;

/// <summary>
/// 本地上传策略
/// </summary>
public class LocalStrategy : Strategy
{
    public override async Task<string> Upload(List<IFormFile> files)
    {
        var res = await Task.Run(() =>
        {
            var result = new List<string>();
            foreach (var formFile in files)
            {
                if (formFile.Length <= 0) continue;
                var filePath = $"{AppContext.BaseDirectory}/wwroot";
                var fileName = $"/{DateTime.Now:yyyyMMddHHmmssfff}{formFile.FileName}";
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                using (var stream = new FileStream(filePath + fileName, FileMode.Create))
                {
                    formFile.CopyTo(stream);
                }

                result.Add(fileName);
            }

            return string.Join(",", result);
        });
        return res;
    }
}