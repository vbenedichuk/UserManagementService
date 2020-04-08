using System.Security.Cryptography.X509Certificates;
using UserManagementService.Data;
using UserManagementService.Mapping;
using UserManagementService.Models.Database;
using UserManagementService.Services;
using UserManagementService.Configuration;
using UserManagementService.Models.Configuration;
using AutoMapper;
using IdentityServer4;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace UserManagementService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var certificateConfiguration = Configuration.GetSection(nameof(CertificateConfiguration));
            var fileName = certificateConfiguration.GetValue<string>(nameof(CertificateConfiguration.FileName));
            var password = certificateConfiguration.GetValue<string>(nameof(CertificateConfiguration.Password));
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });
            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddControllers().AddJsonOptions(opt => opt.JsonSerializerOptions.PropertyNamingPolicy = null);
            services.AddControllersWithViews().AddJsonOptions(opt => opt.JsonSerializerOptions.PropertyNamingPolicy = null);

            services.Configure<CookieAuthenticationOptions>(IdentityServerConstants.DefaultCookieAuthenticationScheme, options =>
            {
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                options.Cookie.IsEssential = true;
            });

            // configures IIS out-of-proc settings (see https://github.com/aspnet/AspNetCore/issues/14882)
            services.Configure<IISOptions>(iis =>
            {
                iis.AuthenticationDisplayName = "Windows";
                iis.AutomaticAuthentication = false;
            });

            // configures IIS in-proc settings
            services.Configure<IISServerOptions>(iis =>
            {
                iis.AuthenticationDisplayName = "Windows";
                iis.AutomaticAuthentication = false;
            });

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
               {
                   options.Authority = "https://localhost:5001";
                   options.RequireHttpsMetadata = false;
                   options.Audience = "UserManagement";
               });

            services.AddTransient<IReturnUrlParser, ReturnUrlParser>();
            services.AddTransient<IPersistedGrantStore, PersistedGrantStore>();
            services.AddCors(setup =>
            {
                setup.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyHeader();
                    policy.AllowAnyMethod();
                    policy.WithOrigins("http://127.0.0.1:8080", "http://localhost:8080", "http://localhost:8082", "http://localhost:4200");
                    policy.AllowCredentials();
                });
            });

            var builder = services.AddIdentityServer(
                options => {
                    options.UserInteraction.LoginUrl = "/Account/Login"; //"http://localhost:8082/index.html"; //"/api/auth/login";
                    options.UserInteraction.ErrorUrl = "/api/auth/error";
                    options.UserInteraction.LogoutUrl = "/api/auth/logout";
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseSuccessEvents = true;
                })
                .AddInMemoryIdentityResources(IdentityServerConfigurationHelper.GetIdentityResources())
                .AddInMemoryApiResources(IdentityServerConfigurationHelper.GetApis())
                .AddInMemoryClients(IdentityServerConfigurationHelper.GetClients())
                .AddAspNetIdentity<ApplicationUser>();
            builder.AddSigningCredential(new X509Certificate2(fileName, password));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
                context.Database.Migrate();
            }


            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseCors();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseIdentityServer();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
