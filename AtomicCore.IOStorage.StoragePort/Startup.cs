using AtomicCore.IOStorage.StoragePort.GrpcService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace AtomicCore.IOStorage.StoragePort
{
    /// <summary>
    /// NetCore Startup
    /// </summary>
    public class Startup
    {
        #region Variabels

        /// <summary>
        /// 跨域策略名称
        /// </summary>
        private const string CORS_POLICYNAME = "AnyCors";

        #endregion

        #region Constructor

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="configuration">系统配置</param>
        /// <param name="env">WebHost变量</param>
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            WebHostEnvironment = env;
        }

        #endregion

        #region Propertys

        /// <summary>
        /// 系统配置
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// WebHost变量
        /// </summary>
        public IWebHostEnvironment WebHostEnvironment { get; }

        #endregion

        #region Public Methods

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            #region AtomicCore引擎初始化

            AtomicCore.AtomicKernel.Initialize();

            #endregion

            #region Appsetting配置参数实例化（单例模型）

            services.AddSingleton(BizIOStorageConfig.Create(Configuration));

            #endregion

            #region PathSrv接口注入

            services.AddSingleton<IBizPathSrvProvider, BizPathSrvProvider>();

            #endregion

            #region 运行环境部署（Linux or IIS）+(文件流最大上线设置为50M)

            //如果部署在linux系统上，需要加上下面的配置：
            //https://docs.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel/options?view=aspnetcore-5.0#maximum-client-connections
            services.Configure<KestrelServerOptions>(options =>
            {
                //options.AllowSynchronousIO = true;

                // Handle requests up to 50 MB
                options.Limits.MaxRequestBodySize = 52428800;
            });

            //////如果部署在IIS上，需要加上下面的配置：
            ////services.Configure<IISServerOptions>(options =>
            ////{
            ////    //options.AllowSynchronousIO = true;

            ////    // Handle requests up to 50 MB
            ////    options.MaxRequestBodySize = 52428800;
            ////});

            #endregion

            #region 设置上传最大限制阀值（修改默认阀值）

            services.Configure<FormOptions>(options =>
            {
                options.ValueLengthLimit = int.MaxValue;
                options.MultipartBodyLengthLimit = long.MaxValue;
                options.MultipartHeadersLengthLimit = int.MaxValue;
            });

            #endregion

            #region Session、IHttpContextAccessor、MVC、Cookie

            /* 《调试模式 -> 若是调试模式,开始razor运行时编译模式》 */
            if (this.WebHostEnvironment.IsDevelopment())
                services.AddRazorPages().AddRazorRuntimeCompilation();

            /* 《启用通用中间件服务》 */
            services.AddHttpContextAccessor();                              // 注册当前线程全生命周期上下文接口调用
            services.AddOptions();                                          // 配置Options模式

            /* 《与 HttpClientFactory 之间的池 HTTP 连接,它会处理池 HTTP 连接以优化性能和可靠性》 */
            services.AddHttpClient();

            /* 《数据保护》 */
            services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(
                this.WebHostEnvironment.ContentRootPath +
                Path.DirectorySeparatorChar +
                "DataProtection"
            ));

            /* 《全局跨域设置》 */
            services.AddCors(option => option.AddPolicy(CORS_POLICYNAME,
                policy => policy
                    .SetIsOriginAllowed(_ => true)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                )
            );

            /*
             * 《MVC相关中间件》
             * 1.注册MVC控制器和视图
             * 2.支持NewtonsoftJson
             * 3.设置MVC版本
             */
            services.AddControllersWithViews(options =>
            {
                /*
                *  1.设置全局拦截(管理后台的请求拦截)
                *  2.修改控制器模型绑定规则
                */
                options.Filters.Add<BizPermissionTokenAttribute>();
            })
            .AddNewtonsoftJson(options =>
            {
                /*
                *  支持 NewtonsoftJson
                *  首字母大写 -> DefaultContractResolver
                *  首字母小写 -> CamelCasePropertyNamesContractResolver
                */
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            });

            /* 
             * 《启用GRPC》 
             *  https://codingnote.cc/p/598478/
             *  https://learn.microsoft.com/en-us/aspnet/core/grpc/test-tools?view=aspnetcore-5.0
             */
            services.AddGrpc();
            services.AddGrpcReflection();

            /* GZIP压缩配置 */
            services.Configure<GzipCompressionProviderOptions>(options =>
                options.Level = System.IO.Compression.CompressionLevel.Optimal
            );
            services.AddResponseCompression(options =>
            {
                options.Providers.Add<GzipCompressionProvider>();
            });

            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            /* 始终输出调试日志 */
            app.UseDeveloperExceptionPage();

            /* 
             * 开启GZIP
             * For safety sake, just keep UseResponseCompression as the first middleware in your list
             */
            app.UseResponseCompression();

            /* 读取Mime拓展配置 */
            IDictionary<string, string> mimeDic = BizMIMETypeConfig.ResolveTypes(this.Configuration);

            /* 激活静态资源访问(调试模式不缓存Cache) */
            var options = new FileServerOptions();
            options.StaticFileOptions.OnPrepareResponse = SetCacheControl;
            var contentTypeProvider = new FileExtensionContentTypeProvider();
            if (null != mimeDic && mimeDic.Count > 0)
                foreach (var kv in mimeDic)
                    if (!contentTypeProvider.Mappings.ContainsKey(kv.Key))
                        contentTypeProvider.Mappings.Add(kv);
            options.StaticFileOptions.ContentTypeProvider = contentTypeProvider;
            //options.EnableDefaultFiles = true;
            //options.DefaultFilesOptions.DefaultFileNames = new List<string>
            //{
            //    "index.html",
            //    "index.htm"
            //};
            app.UseFileServer(options);

            /* 按顺序启动激活Mvc相关中间件 */
            app.UseCors(CORS_POLICYNAME);                   //激活全局跨域
            app.UseRouting();                               //激活Routing
            //app.UseAuthorization();                         //激活认证服务
            app.UseResponseCaching();                       //激活输出缓存
            app.UseEndpoints(endpoints =>
            {
                // 注册GRPC服务,并且打开调试引用（在调试状态下）
                endpoints.MapGrpcService<BizFileGrpcService>();
                endpoints.MapGrpcReflectionService();

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 设置cache control
        /// </summary>
        /// <param name="context"></param>
        private static void SetCacheControl(StaticFileResponseContext context)
        {
            // Format RFC1123
            int second = 365 * 24 * 60 * 60;
            context.Context.Response.Headers.Append("Cache-Control", new[] { "public,max-age=" + second });
            context.Context.Response.Headers.Append("Expires", new[] { DateTime.UtcNow.AddYears(1).ToString("R") });

            // 允许跨域
            context.Context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
        }

        #endregion
    }
}
