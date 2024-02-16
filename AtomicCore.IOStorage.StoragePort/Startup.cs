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
        /// �����������
        /// </summary>
        private const string CORS_POLICYNAME = "AnyCors";

        #endregion

        #region Constructor

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="configuration">ϵͳ����</param>
        /// <param name="env">WebHost����</param>
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            WebHostEnvironment = env;
        }

        #endregion

        #region Propertys

        /// <summary>
        /// ϵͳ����
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// WebHost����
        /// </summary>
        public IWebHostEnvironment WebHostEnvironment { get; }

        #endregion

        #region Public Methods

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            #region AtomicCore�����ʼ��

            AtomicCore.AtomicKernel.Initialize();

            #endregion

            #region Appsetting���ò���ʵ����������ģ�ͣ�

            services.AddSingleton(BizIOStorageConfig.Create(Configuration));

            #endregion

            #region PathSrv�ӿ�ע��

            services.AddSingleton<IBizPathSrvProvider, BizPathSrvProvider>();

            #endregion

            #region ���л�������Linux or IIS��+(�ļ��������������Ϊ50M)

            //���������linuxϵͳ�ϣ���Ҫ������������ã�
            //https://docs.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel/options?view=aspnetcore-5.0#maximum-client-connections
            services.Configure<KestrelServerOptions>(options =>
            {
                //options.AllowSynchronousIO = true;

                // Handle requests up to 50 MB
                options.Limits.MaxRequestBodySize = 52428800;
            });

            //////���������IIS�ϣ���Ҫ������������ã�
            ////services.Configure<IISServerOptions>(options =>
            ////{
            ////    //options.AllowSynchronousIO = true;

            ////    // Handle requests up to 50 MB
            ////    options.MaxRequestBodySize = 52428800;
            ////});

            #endregion

            #region �����ϴ�������Ʒ�ֵ���޸�Ĭ�Ϸ�ֵ��

            services.Configure<FormOptions>(options =>
            {
                options.ValueLengthLimit = int.MaxValue;
                options.MultipartBodyLengthLimit = long.MaxValue;
                options.MultipartHeadersLengthLimit = int.MaxValue;
            });

            #endregion

            #region Session��IHttpContextAccessor��MVC��Cookie

            /* ������ģʽ -> ���ǵ���ģʽ,��ʼrazor����ʱ����ģʽ�� */
            if (this.WebHostEnvironment.IsDevelopment())
                services.AddRazorPages().AddRazorRuntimeCompilation();

            /* ������ͨ���м������ */
            services.AddHttpContextAccessor();                              // ע�ᵱǰ�߳�ȫ�������������Ľӿڵ���
            services.AddOptions();                                          // ����Optionsģʽ

            /* ���� HttpClientFactory ֮��ĳ� HTTP ����,���ᴦ��� HTTP �������Ż����ܺͿɿ��ԡ� */
            services.AddHttpClient();

            /* �����ݱ����� */
            services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(
                this.WebHostEnvironment.ContentRootPath +
                Path.DirectorySeparatorChar +
                "DataProtection"
            ));

            /* ��ȫ�ֿ������á� */
            services.AddCors(option => option.AddPolicy(CORS_POLICYNAME,
                policy => policy
                    .SetIsOriginAllowed(_ => true)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                )
            );

            /*
             * ��MVC����м����
             * 1.ע��MVC����������ͼ
             * 2.֧��NewtonsoftJson
             * 3.����MVC�汾
             */
            services.AddControllersWithViews(options =>
            {
                /*
                *  1.����ȫ������(�����̨����������)
                *  2.�޸Ŀ�����ģ�Ͱ󶨹���
                */
                options.Filters.Add<BizPermissionTokenAttribute>();
            })
            .AddNewtonsoftJson(options =>
            {
                /*
                *  ֧�� NewtonsoftJson
                *  ����ĸ��д -> DefaultContractResolver
                *  ����ĸСд -> CamelCasePropertyNamesContractResolver
                */
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            });

            /* 
             * ������GRPC�� 
             *  https://codingnote.cc/p/598478/
             *  https://learn.microsoft.com/en-us/aspnet/core/grpc/test-tools?view=aspnetcore-5.0
             */
            services.AddGrpc();
            services.AddGrpcReflection();

            /* GZIPѹ������ */
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
            /* ʼ�����������־ */
            app.UseDeveloperExceptionPage();

            /* 
             * ����GZIP
             * For safety sake, just keep UseResponseCompression as the first middleware in your list
             */
            app.UseResponseCompression();

            /* ��ȡMime��չ���� */
            IDictionary<string, string> mimeDic = BizMIMETypeConfig.ResolveTypes(this.Configuration);

            /* ���̬��Դ����(����ģʽ������Cache) */
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

            /* ��˳����������Mvc����м�� */
            app.UseCors(CORS_POLICYNAME);                   //����ȫ�ֿ���
            app.UseRouting();                               //����Routing
            //app.UseAuthorization();                         //������֤����
            app.UseResponseCaching();                       //�����������
            app.UseEndpoints(endpoints =>
            {
                // ע��GRPC����,���Ҵ򿪵������ã��ڵ���״̬�£�
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
        /// ����cache control
        /// </summary>
        /// <param name="context"></param>
        private static void SetCacheControl(StaticFileResponseContext context)
        {
            // Format RFC1123
            int second = 365 * 24 * 60 * 60;
            context.Context.Response.Headers.Append("Cache-Control", new[] { "public,max-age=" + second });
            context.Context.Response.Headers.Append("Expires", new[] { DateTime.UtcNow.AddYears(1).ToString("R") });

            // �������
            context.Context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
        }

        #endregion
    }
}
