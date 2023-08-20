namespace AtomicCore.IOStorage.StoragePort
{
    /// <summary>
    /// 路径接口
    /// </summary>
    public interface IBizPathSrvProvider
    {
        /// <summary>
        /// appToken密钥
        /// </summary>
        string AppToken { get; }

        /// <summary>
        /// 文件存储根目录
        /// </summary>
        string SaveRootDir { get; }

        /// <summary>
        /// 允许存储的格式（eg -> .jpg .png ....）
        /// </summary>
        string[] PermittedExtensions { get; }

        /// <summary>
        /// 允许存储的单文件最大限制（单位:B）
        /// </summary>
        long FileSizeLimit { get; }

        /// <summary>
        /// 获取IO路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        string MapPath(string path);
    }
}
