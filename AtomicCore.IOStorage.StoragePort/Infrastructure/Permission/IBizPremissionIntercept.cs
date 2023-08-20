using Microsoft.AspNetCore.Mvc;

namespace AtomicCore.IOStorage.StoragePort
{
    /// <summary>
    /// 权限拦截接口
    /// </summary>
    public interface IBizPremissionIntercept
    {
        /// <summary>
        /// 拦截处理方法
        /// </summary>
        /// <param name="requestContext"></param>
        void OnIntercept(ActionContext requestContext);
    }
}
