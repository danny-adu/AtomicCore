using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using System;

namespace AtomicCore.IOStorage.StoragePort.Controllers
{
    /// <summary>
    /// MVC控制器抽象类
    /// </summary>
    public abstract class BizControllerBase : Controller, IBizPremissionIntercept
    {
        #region Variables

        /// <summary>
        /// 头部信息Token
        /// </summary>
        private const string c_head_token = "token";

        #endregion

        #region Public Propertys

        /// <summary>
        /// 是否有权限
        /// </summary>
        public bool HasPremission { get; private set; } = true;

        #endregion

        #region Public Methods

        /// <summary>
        /// 请求拦截处理
        /// </summary>
        /// <param name="requestContext"></param>
        public void OnIntercept(ActionContext requestContext)
        {
            //判断接口实例是否获取的到
            IBizPathSrvProvider pathSrvProvider = requestContext.HttpContext.RequestServices.GetService<IBizPathSrvProvider>();
            if (null == pathSrvProvider)
            {
                Console.WriteLine($"--> '{nameof(IBizPathSrvProvider)}' is null, are you register the interface of '{nameof(IBizPathSrvProvider)}' in startup?");
                this.HasPremission = false;
                return;
            }
            if(string.IsNullOrEmpty(pathSrvProvider.AppToken))
            {
                Console.WriteLine($"--> '{nameof(pathSrvProvider.AppToken)}' is null, are you setting the env or appsetting?");
                this.HasPremission = false;
                return;
            }

            //判断头部是否包含token
            bool hasHeadToken = requestContext.HttpContext.Request.Headers.TryGetValue(c_head_token, out StringValues headTK);
            if (!hasHeadToken)
                this.HasPremission = false;
            else
            {
                this.HasPremission = pathSrvProvider.AppToken.Equals(headTK.ToString(), StringComparison.OrdinalIgnoreCase);

                if (!this.HasPremission)
                    Console.WriteLine($"--> app token is illegal, current request token is '{headTK}'");
            }
        }

        #endregion
    }
}
