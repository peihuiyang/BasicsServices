using Autofac;
using BasicsServices.Api.Configuration.Helper;
using BasicsServices.Api.Configuration.Swagger;
using BasicsServices.Api.Filters;
using BasicsServices.Api.RequestBody;
using Hangfire;
using Hangfire.Console;
using Hangfire.Dashboard;
using Hangfire.Dashboard.BasicAuthorization;
using Hangfire.HttpJob;
using Hangfire.Redis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Peihui.Common.Base.Security;
using Peihui.Common.JsonHelper;
using Peihui.Core.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TimeZoneConverter;

namespace BasicsServices.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        /// <summary>
        /// 程序集列表
        /// </summary>
        private static readonly List<string> _Assemblies = new List<string>()
        {
            "BasicsServices.ApplicationService",
            "BasicsServices.DomainService",
            "BasicsServices.Repository"
        };
        /// <summary>
        /// 依赖注入
        /// </summary>
        /// <param name="container"></param>
        public void ConfigureContainer(ContainerBuilder container)
        {
            #region AutoFac
            //container.RegisterType<GridFSHelper>().As<IGridFSHelper>().AsImplementedInterfaces();
            #endregion
            var assemblys = _Assemblies.Select(x => Assembly.Load(x)).ToList();
            List<Type> allTypes = new List<Type>();
            assemblys.ForEach(aAssembly =>
            {
                allTypes.AddRange(aAssembly.GetTypes());
            });

            // 通过Autofac自动完成依赖注入
            container.RegisterTypes(allTypes.ToArray())
                .AsImplementedInterfaces()
                .PropertiesAutowired()
                .InstancePerDependency();

            //// 注册Controller
            container.RegisterAssemblyTypes(typeof(Startup).GetTypeInfo().Assembly)
                .Where(t => typeof(Controller).IsAssignableFrom(t) && t.Name.EndsWith("Controller", StringComparison.Ordinal))
                .PropertiesAutowired();
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region 处理多请求头
            services.AddMvc(o => o.InputFormatters.Insert(0, new RawRequestBodyFormatter())).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            #endregion

            #region 全局异常捕获

            // =>注册MVC到Container,同时添加全局过滤器
            services.AddMvc(options =>
            {
                options.Filters.Add<HttpGlobalExceptionFilter>();

                // =>请求返回结果捕获
                //options.Filters.Add<>(HttpResultFilter);
            }).AddJsonOptions(
                // =>json序列化配置
                option =>
                {
                    // =>对 JSON 数据使用混合大小写。驼峰式首字母小写形式输出.
                    //option.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
                    // =>对 JSON 数据使用混合大小写。跟属性名同样的大小输出.
                    //option.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
                    // =>Json序列化属性名全部小写
                    //option.SerializerSettings.ContractResolver = new LowercaseContractResolver();
                    //配置序列化时时间格式为yyyy-MM-dd HH:mm:ss
                    option.JsonSerializerOptions.Converters.Add(new Configuration.JsonOptions.DateTimeConverter());
                }
                );

            #endregion

            #region 配置Swagger

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "基础服务中心 WebApi",
                    Version = "v1",
                    Description = "提供系统所需的基础服务",
                    TermsOfService = new Uri("http://47.106.73.201:8080/"),
                    Contact = new OpenApiContact()
                    {
                        Name = "杨培辉",
                        Email = "1293035342@qq.com"
                    }
                });
                // 加Token验证
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Token登录验证,格式：Bearer {token}(注意两者之间是一个空格)",
                    Name = "Authorization",
                    //这两个参数均有修改
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });

                // =>设置xml注释文档，注意名称一定要与项目名称相同
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var apiXmlPath = Path.Combine(basePath, "BasicsServices.Api.xml");
                var entityDtoXmlPath = Path.Combine(basePath, "BasicsServices.EntityDto.xml");
                c.IncludeXmlComments(apiXmlPath);
                c.IncludeXmlComments(entityDtoXmlPath);

                // =>解决生成Api文档时，类名相同，命名空间不同时报错的问题
                c.CustomSchemaIds(x => x.FullName);
                //c.DescribeAllEnumsAsStrings();
                c.DocumentFilter<SwaggerControllerTag>();
            });

            #endregion

            #region HttpClient
            services.AddHttpClient();
            #endregion

            services.AddControllers()

            #region Controller DI And IOC
                .AddControllersAsServices()
            #endregion

            #region NewtonsoftJson
                .AddNewtonsoftJson(options =>
                {
                    //修改属性名称的序列化方式，首字母小写
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

                    //修改时间的序列化方式
                    options.SerializerSettings.Converters.Add(new IsoDateTimeConverter() { DateTimeFormat = "yyyy/MM/dd HH:mm:ss" });
                });
            #endregion

            services.AddHangfire(HangfireConfiguration);
            services.AddHangfireServer(HangfirJobOptions);
        }
        #region Hangfire配置
        /// <summary>
        /// Hangfire配置
        /// </summary>
        /// <param name="globalConfiguration"></param>
        private void HangfireConfiguration(IGlobalConfiguration globalConfiguration)
        {
            //globalConfiguration.UseStorage(new SqlServerStorage("HangfireConnectionString", new SqlServerStorageOptions
            //{

            //}));

            globalConfiguration
                            .UseRedisStorage(ConnConfig.GetConnectionString("Timing_Task"), new RedisStorageOptions
                            {
                                // 成功列表中的最大可见后台作业
                                SucceededListSize = 10000,
                                // 删除列表中的最大可见后台作业
                                DeletedListSize = 5000,
                                InvisibilityTimeout = TimeSpan.FromHours(3),
                                // 任务过期检查频率
                                ExpiryCheckInterval = TimeSpan.FromHours(1),
                                Prefix = "hangfire:", // 前缀
                                Db = 10
                            })
                            .UseConsole(new ConsoleOptions()
                            {
                                BackgroundColor = "#A9F5D0"
                            })
                            .UseHangfireHttpJob(new HangfireHttpJobOptions
                            {
                                AddHttpJobButtonName = "添加计划任务",
                                AddRecurringJobHttpJobButtonName = "添加定时任务",
                                EditRecurringJobButtonName = "编辑定时任务",
                                PauseJobButtonName = "暂停或开始",
                                DashboardTitle = "任务管理",
                                DashboardName = "后台任务管理",
                                DashboardFooter = "后台任务管理V1.0.0.0",
                                // 配置通知邮箱
                                MailOption = new MailOption
                                {
                                    Server = JsonConfigHelper.Configuration[string.Format("HangfireMail:Server")],
                                    Port = Convert.ToInt32(JsonConfigHelper.Configuration[string.Format("HangfireMail:Port")]),
                                    UseSsl = Convert.ToBoolean(JsonConfigHelper.Configuration[string.Format("HangfireMail:UseSsl")]),
                                    User = JsonConfigHelper.Configuration[string.Format("HangfireMail:User")],
                                    Password = AesHelper.Decrypt(JsonConfigHelper.Configuration[string.Format("HangfireMail:Password")]),
                                },
                                DefaultRecurringQueueName = "default",
                                DefaultBackGroundJobQueueName = "default",
                                // RecurringJobTimeZone = TZConvert.GetTimeZoneInfo("Asia/Shanghai"), //这里指定了添加周期性job时的时区
                                // RecurringJobTimeZone = TimeZoneInfo.Local
                                // CheckHttpResponseStatusCode = code => (int)code < 400   //===》(default)
                            })
                            .UseDashboardMetric(DashboardMetrics.AwaitingCount)
                            .UseDashboardMetric(DashboardMetrics.ProcessingCount)
                            .UseDashboardMetric(DashboardMetrics.RecurringJobCount)
                            .UseDashboardMetric(DashboardMetrics.RetriesCount)
                            .UseDashboardMetric(DashboardMetrics.FailedCount)
                            .UseDashboardMetric(DashboardMetrics.ServerCount);
        }

        /// <summary>
        /// 服务器任务配置
        /// </summary>
        /// <param name="backgroundJobServerOptions"></param>
        private void HangfirJobOptions(BackgroundJobServerOptions backgroundJobServerOptions)
        {
            backgroundJobServerOptions.ServerTimeout = TimeSpan.FromMinutes(4);
            backgroundJobServerOptions.SchedulePollingInterval = TimeSpan.FromSeconds(1);//秒级任务需要配置短点，一般任务可以配置默认时间，默认15秒
            backgroundJobServerOptions.ShutdownTimeout = TimeSpan.FromMinutes(30);//超时时间
            backgroundJobServerOptions.Queues = new[] { "default", "apis", "recurring" };//队列
            backgroundJobServerOptions.WorkerCount = Math.Max(Environment.ProcessorCount, 40);//工作线程数，当前允许的最大线程，默认20
            backgroundJobServerOptions.HeartbeatInterval = TimeSpan.FromMinutes(1);
            backgroundJobServerOptions.ServerName = "YPH的定时任务平台";
        }
        #endregion
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            #region Swagger

            // =>启用Swagger中间件以生成Swagger作为JSON数据.(一定要放在app.UseMvc()之前)
            app.UseSwagger();
            // =>启用SwaggerUI中间件，数据采用swagger.json
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "BasicsServices WebApi V1");
                c.RoutePrefix = string.Empty;
            });

            #endregion

            #region 支持nginx
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            #endregion

            #region Hangfire
            //app.UseHangfireServer(new BackgroundJobServerOptions()
            //{
            //    ServerTimeout = TimeSpan.FromMinutes(4),
            //    SchedulePollingInterval = TimeSpan.FromSeconds(1),//秒级任务需要配置短点，一般任务可以配置默认时间，默认15秒
            //    ShutdownTimeout = TimeSpan.FromMinutes(30),//超时时间
            //    Queues = new[] { "default", "apis", "recurring" },//队列
            //    WorkerCount = Math.Max(Environment.ProcessorCount, 40)//工作线程数，当前允许的最大线程，默认20
            //});
            app.UseHangfireDashboard("/hangfire/index", new DashboardOptions
            {
                Authorization = new[] { new BasicAuthAuthorizationFilter(new BasicAuthAuthorizationFilterOptions
                {
                    RequireSsl = false,
                    SslRedirect = false,
                    LoginCaseSensitive = true,
                    Users = new []
                    {
                        new BasicAuthAuthorizationUser
                        {
                            Login = "admin",
                            PasswordClear =  "adminispeihui"
                        }
                    }
                }) }
            });
            #endregion

            #region 注册Consul服务
            this.Configuration.ConsulRegister();
            #endregion

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
