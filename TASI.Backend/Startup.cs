using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TASI.Backend.Infrastructure.Configs;
using TASI.Backend.Infrastructure.Database;
using TASI.Backend.Infrastructure.Registrations;

namespace TASI.Backend
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
            services.AddOptions();
            services.AddMemoryCache();
            services.AddAutoMapper(typeof(ObjectMapperProfile));
            services.AddMediatR(Assembly.GetExecutingAssembly());

            services.AddDbContext<TasiContext>(options => options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));
            services.AddDatabaseDeveloperPageExceptionFilter();
            
            services.AddControllers()
                .AddCustomNewtonsoftJson()
                .AddCustomValidator();
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddCustomCors();
            services.AddCustomAuth(Configuration);
            services.AddCustomSwagger("TASI Backend API", "v1");
            services.AddCustomHealthChecks(Configuration);
       
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
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
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseCustomSerilog();

            app.UseCustomSwagger("TASI Backend API", "v1");
            app.UseCustomHealthChecks(env);
            app.UseCustomResponseWrapper(env);

            app.UseRouting();
            app.UseCustomCors();
            app.UseCustomAuth();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapCustomHealthChecks();
                endpoints.MapControllers();
            });
        }
    }
}
