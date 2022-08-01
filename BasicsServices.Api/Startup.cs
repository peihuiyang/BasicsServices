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
        /// �����б�
        /// </summary>
        private static readonly List<string> _Assemblies = new List<string>()
        {
            "BasicsServices.ApplicationService",
            "BasicsServices.DomainService",
            "BasicsServices.Repository"
        };
        /// <summary>
        /// ����ע��
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

            // ͨ��Autofac�Զ��������ע��
            container.RegisterTypes(allTypes.ToArray())
                .AsImplementedInterfaces()
                .PropertiesAutowired()
                .InstancePerDependency();

            //// ע��Controller
            container.RegisterAssemblyTypes(typeof(Startup).GetTypeInfo().Assembly)
                .Where(t => typeof(Controller).IsAssignableFrom(t) && t.Name.EndsWith("Controller", StringComparison.Ordinal))
                .PropertiesAutowired();
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region ���������ͷ
            services.AddMvc(o => o.InputFormatters.Insert(0, new RawRequestBodyFormatter())).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            #endregion

            #region ȫ���쳣����

            // =>ע��MVC��Container,ͬʱ���ȫ�ֹ�����
            services.AddMvc(options =>
            {
                options.Filters.Add<HttpGlobalExceptionFilter>();

                // =>���󷵻ؽ������
                //options.Filters.Add<>(HttpResultFilter);
            }).AddJsonOptions(
                // =>json���л�����
                option =>
                {
                    // =>�� JSON ����ʹ�û�ϴ�Сд���շ�ʽ����ĸСд��ʽ���.
                    //option.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
                    // =>�� JSON ����ʹ�û�ϴ�Сд����������ͬ���Ĵ�С���.
                    //option.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
                    // =>Json���л�������ȫ��Сд
                    //option.SerializerSettings.ContractResolver = new LowercaseContractResolver();
                    //�������л�ʱʱ���ʽΪyyyy-MM-dd HH:mm:ss
                    option.JsonSerializerOptions.Converters.Add(new Configuration.JsonOptions.DateTimeConverter());
                }
                );

            #endregion

            #region ����Swagger

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "������������ WebApi",
                    Version = "v1",
                    Description = "�ṩϵͳ����Ļ�������",
                    TermsOfService = new Uri("http://47.106.73.201:8080/"),
                    Contact = new OpenApiContact()
                    {
                        Name = "�����",
                        Email = "1293035342@qq.com"
                    }
                });
                // ��Token��֤
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Token��¼��֤,��ʽ��Bearer {token}(ע������֮����һ���ո�)",
                    Name = "Authorization",
                    //���������������޸�
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });

                // =>����xmlע���ĵ���ע������һ��Ҫ����Ŀ������ͬ
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var apiXmlPath = Path.Combine(basePath, "BasicsServices.Api.xml");
                var entityDtoXmlPath = Path.Combine(basePath, "BasicsServices.EntityDto.xml");
                c.IncludeXmlComments(apiXmlPath);
                c.IncludeXmlComments(entityDtoXmlPath);

                // =>�������Api�ĵ�ʱ��������ͬ�������ռ䲻ͬʱ���������
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
                    //�޸��������Ƶ����л���ʽ������ĸСд
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

                    //�޸�ʱ������л���ʽ
                    options.SerializerSettings.Converters.Add(new IsoDateTimeConverter() { DateTimeFormat = "yyyy/MM/dd HH:mm:ss" });
                });
            #endregion

            services.AddHangfire(HangfireConfiguration);
            services.AddHangfireServer(HangfirJobOptions);
        }
        #region Hangfire����
        /// <summary>
        /// Hangfire����
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
                                // �ɹ��б��е����ɼ���̨��ҵ
                                SucceededListSize = 10000,
                                // ɾ���б��е����ɼ���̨��ҵ
                                DeletedListSize = 5000,
                                InvisibilityTimeout = TimeSpan.FromHours(3),
                                // ������ڼ��Ƶ��
                                ExpiryCheckInterval = TimeSpan.FromHours(1),
                                Prefix = "hangfire:", // ǰ׺
                                Db = 10
                            })
                            .UseConsole(new ConsoleOptions()
                            {
                                BackgroundColor = "#A9F5D0"
                            })
                            .UseHangfireHttpJob(new HangfireHttpJobOptions
                            {
                                AddHttpJobButtonName = "��Ӽƻ�����",
                                AddRecurringJobHttpJobButtonName = "��Ӷ�ʱ����",
                                EditRecurringJobButtonName = "�༭��ʱ����",
                                PauseJobButtonName = "��ͣ��ʼ",
                                DashboardTitle = "�������",
                                DashboardName = "��̨�������",
                                DashboardFooter = "��̨�������V1.0.0.0",
                                // ����֪ͨ����
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
                                // RecurringJobTimeZone = TZConvert.GetTimeZoneInfo("Asia/Shanghai"), //����ָ�������������jobʱ��ʱ��
                                // RecurringJobTimeZone = TimeZoneInfo.Local
                                // CheckHttpResponseStatusCode = code => (int)code < 400   //===��(default)
                            })
                            .UseDashboardMetric(DashboardMetrics.AwaitingCount)
                            .UseDashboardMetric(DashboardMetrics.ProcessingCount)
                            .UseDashboardMetric(DashboardMetrics.RecurringJobCount)
                            .UseDashboardMetric(DashboardMetrics.RetriesCount)
                            .UseDashboardMetric(DashboardMetrics.FailedCount)
                            .UseDashboardMetric(DashboardMetrics.ServerCount);
        }

        /// <summary>
        /// ��������������
        /// </summary>
        /// <param name="backgroundJobServerOptions"></param>
        private void HangfirJobOptions(BackgroundJobServerOptions backgroundJobServerOptions)
        {
            backgroundJobServerOptions.ServerTimeout = TimeSpan.FromMinutes(4);
            backgroundJobServerOptions.SchedulePollingInterval = TimeSpan.FromSeconds(1);//�뼶������Ҫ���ö̵㣬һ�������������Ĭ��ʱ�䣬Ĭ��15��
            backgroundJobServerOptions.ShutdownTimeout = TimeSpan.FromMinutes(30);//��ʱʱ��
            backgroundJobServerOptions.Queues = new[] { "default", "apis", "recurring" };//����
            backgroundJobServerOptions.WorkerCount = Math.Max(Environment.ProcessorCount, 40);//�����߳�������ǰ���������̣߳�Ĭ��20
            backgroundJobServerOptions.HeartbeatInterval = TimeSpan.FromMinutes(1);
            backgroundJobServerOptions.ServerName = "YPH�Ķ�ʱ����ƽ̨";
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

            // =>����Swagger�м��������Swagger��ΪJSON����.(һ��Ҫ����app.UseMvc()֮ǰ)
            app.UseSwagger();
            // =>����SwaggerUI�м�������ݲ���swagger.json
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "BasicsServices WebApi V1");
                c.RoutePrefix = string.Empty;
            });

            #endregion

            #region ֧��nginx
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            #endregion

            #region Hangfire
            //app.UseHangfireServer(new BackgroundJobServerOptions()
            //{
            //    ServerTimeout = TimeSpan.FromMinutes(4),
            //    SchedulePollingInterval = TimeSpan.FromSeconds(1),//�뼶������Ҫ���ö̵㣬һ�������������Ĭ��ʱ�䣬Ĭ��15��
            //    ShutdownTimeout = TimeSpan.FromMinutes(30),//��ʱʱ��
            //    Queues = new[] { "default", "apis", "recurring" },//����
            //    WorkerCount = Math.Max(Environment.ProcessorCount, 40)//�����߳�������ǰ���������̣߳�Ĭ��20
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

            #region ע��Consul����
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
