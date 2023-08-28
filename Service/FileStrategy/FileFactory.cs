using Model.Enum;

namespace Service.FileStrategy;

/// <summary>
/// 工厂类，负责创建具体的策略
/// </summary>
public class FileFactory
{
    public static Strategy CreateStrategy(UploadMode mode)
    {
        return mode switch
        {
            UploadMode.Local => new LocalStrategy(),
            UploadMode.QiNiu => new QiNiuStrategy(),
            _ => new QiNiuStrategy()
        };
    }
}