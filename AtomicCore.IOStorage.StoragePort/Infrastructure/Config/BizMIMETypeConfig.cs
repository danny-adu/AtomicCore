using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace AtomicCore.IOStorage.StoragePort
{
    /// <summary>
    /// MIME类型
    /// </summary>
    public class BizMIMETypeConfig
    {
        #region Static Methods

        /// <summary>
        /// 类型解析
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IDictionary<string, string> ResolveTypes(IConfiguration configuration)
        {
            //获取MIME类型
            IConfiguration mimeTypeCfg = configuration.GetSection("MIMEType");
            if (null == mimeTypeCfg)
                return null;

            //获取类型Cfg
            IEnumerable<IConfigurationSection> childSections = mimeTypeCfg.GetChildren();
            if (null == childSections || !childSections.Any())
                return null;

            return childSections.ToDictionary(s => s.GetValue<string>(nameof(s.Key)), v => v.GetValue<string>(nameof(v.Value)));
        }

        #endregion
    }
}
