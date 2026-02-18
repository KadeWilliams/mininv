using HobbyLobby.AsyncHelper;
using HobbyLobby.Avatar.ApplicationSecurity.OpenIdConnect.Configuration;
using HobbyLobby.Avatar.AspNetCore;
using HobbyLobby.Configuration;
using HobbyLobby.Configuration.Extensions;
using HobbyLobby.DocumentManagement;
using HobbyLobby.LibConfig;
using HobbyLobby.Logging;
using HobbyLobby.OpenIdClient.ApiClients;
using HobbyLobby.OpenIdClient.TokenStores;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MinistryInvestment.Core.Configuration;
using MinistryInvestment.Core.DocumentManagement;
using MinistryInvestment.Core.Repositories;
using MinistryInvestment.Core.Services;
using MinistryInvestment.Mvc.ActionFilters;
using MinistryInvestment.Mvc.Configuration;
using MinistryInvestment.Mvc.Configuration.Mapping;
using System;
using System.IO;

namespace MinistryInvestment.Mvc
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }

        public IWebHostEnvironment Environment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            IConfigurationFactory configurationFactory = new ConfigurationFactory();
            configurationFactory.AddHobbyLobbyConfiguration(supportEncryption: true);
            var corsOrigins = Configuration.GetSection("Cors:Origins").Get<string[]>();
            var corsMethods = Configuration.GetSection("Cors:Methods").Get<string[]>();

            services.AddCors(options => options.AddDefaultPolicy(policy =>
                policy.SetIsOriginAllowedToAllowWildcardSubdomains()
                .WithOrigins(corsOrigins)
                .WithMethods(corsMethods)
                .WithHeaders("authorization", "accept", "content-type", "origin"))
            );


            ConfigureLibConfig(services);
            ConfigureLogging(services);
            var avatarClientConfiguration = ConfigureAvatar(services, configurationFactory);
            ConfigureDataProtection(services);
            ConfigureSession(services, avatarClientConfiguration);
            ConfigureAPIClient(services, configurationFactory);
            ConfigureApp(services);
            ConfigureMvc(services);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsEnvironment("Local"))
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = responseContext =>
                {
                    // Cache static assets for one year by default
                    responseContext.Context.Response.Headers.Append("Cache-Control", "public, max-age=31536000");
                }
            });

            app.UseRouting();
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private void ConfigureLibConfig(IServiceCollection services)
        {
            services.AddSingleton<Config>(_ =>
            {
                var config = new Config(Configuration["hobbylobby.libconfig:configName"]);
                config.StartWatchingForConfigChanges();

                return config;
            });
        }

        private void ConfigureLogging(IServiceCollection services)
        {
            services.AddSingleton<ILogFactory>(provider =>
            {
                var logFactory = new LogFactory();
                logFactory.ConfigureWith.LibConfig(provider.GetRequiredService<Config>());

                return logFactory;
            });

            services.AddSingleton<ILog, ChangeTrackingLog>();
        }

        private IClientConfiguration ConfigureAvatar(IServiceCollection services, IConfigurationFactory configurationFactory)
        {
            configurationFactory.AddLibConfigShimForAvatar();
            configurationFactory.AddLibConfigShimForDocumentManagement();

            var avatarClientConfiguration = configurationFactory.CreateConfiguration<IClientConfiguration>();
            services.AddSingleton<IClientConfiguration>(avatarClientConfiguration);

            services.AddAvatarOpenIdConnectAuthentication(avatarClientConfiguration);
            services.AddAvatarOpenIdConnectAuthorization();

            return avatarClientConfiguration;
        }

        private void ConfigureDataProtection(IServiceCollection services)
        {
            if (!Environment.IsEnvironment("Local"))
            {
                services.AddDataProtection()
                    .PersistKeysToFileSystem(new DirectoryInfo(Configuration["dataProtection:fileShare"]))
                    .SetApplicationName(Configuration["dataProtection:applicationName"]);

                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = Configuration["distributedRedisCache:server"];
                    options.InstanceName = Configuration["distributedRedisCache:instance"];
                });
            }
        }

        private void ConfigureSession(IServiceCollection services, IClientConfiguration avatarClientConfiguration)
        {
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(int.Parse(Configuration["session:idleTimeout"]));
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.Name = $"{avatarClientConfiguration.ClientId}.AspNetCore.Session";
            });
        }

        private void ConfigureAPIClient(IServiceCollection services, IConfigurationFactory configurationFactory)
        {
            services.AddScoped<IApiClient>(provider =>
            {
                return new SecureApiClient(
                    provider.GetRequiredService<IAccessTokenBearer>());
            });

            services.AddSingleton<IAccessTokenStore, AccessTokenStore>();

            services.AddScoped<IAccessTokenBearer>(provider =>
            {
                var accessTokenStore = provider.GetRequiredService<IAccessTokenStore>();

                configurationFactory.MapConfiguration(new AvatarAppConfigurationMap());
                var avatarAppConfiguration = configurationFactory.CreateConfiguration<IAvatarAppConfiguration>();
                return SyncWrapper.RunSync(
                    () => accessTokenStore.GetAccessTokenBearerAsync(
                        avatarAppConfiguration.ClientId,
                        avatarAppConfiguration.ClientSecret));
            });

            configurationFactory.MapConfiguration(new AccessTokenStoreConfigurationMap());
            var accessTokenStoreConfiguration = configurationFactory.CreateConfiguration<IAccessTokenStoreConfiguration>();
            services.AddSingleton<IAccessTokenStoreConfiguration>(accessTokenStoreConfiguration);

            configurationFactory.MapConfiguration<IDocumentManagementConfiguration>(new HobbyLobby.DocumentManagement.DocumentManagementConfigurationMap());
            services.AddSingleton<IDocumentManagementConfiguration>(provider => configurationFactory.CreateConfiguration<IDocumentManagementConfiguration>());
            services.AddScoped<IFolderClient, FolderClient>();
            services.AddScoped<IDocumentClient, DocumentClient>();
        }

        private void ConfigureApp(IServiceCollection services)
        {
            services.AddSingleton<IMinistryInvestmentConfig, MinistryInvestmentConfig>();
            services.AddScoped<IMinistryInvestmentService, MinistryInvestmentService>();
            services.AddScoped<IMinistryInvestmentRepository, MinistryInvestmentRepository>();
            services.AddScoped<IDocumentManagementHelper, DocumentManagementHelper>();
        }

        private void ConfigureMvc(IServiceCollection services)
        {
            services.AddHealthChecks();
            services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

            var builder = services.AddControllersWithViews(options =>
            {
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                options.Filters.Add(new ActionFilter());
                options.EnableAvatarOpenIdConnectAuthorization();
            });
        }
    }
}