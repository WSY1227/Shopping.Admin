using Microsoft.AspNetCore.Http;

namespace Service.FileStrategy;

public class QiNiuStrategy:Strategy
{
    public override async Task<string> Upload(List<IFormFile> files)
    {
        return await Task.Run(() => "QiNiu");
    }
}