namespace Dapper.Extensions
{
    /// <summary>
    /// 数据库连接解密驱动接口
    /// </summary>
    public interface IDecryptProvider
    {
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="encrypted">密文</param>
        /// <returns>原文</returns>
        string Decrypt(string encrypted);
    }
}
