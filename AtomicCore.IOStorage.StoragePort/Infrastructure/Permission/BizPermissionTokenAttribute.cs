using Microsoft.AspNetCore.Mvc.Filters;

namespace AtomicCore.IOStorage.StoragePort
{
    /// <summary>
    /// 权限拦截标签
    /// </summary>
    public class BizPermissionTokenAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 执行Action拦截
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.Controller is IBizPremissionIntercept intercept)
            {
                intercept.OnIntercept(filterContext);
                return;
            }

            return;
        }
    }
}
